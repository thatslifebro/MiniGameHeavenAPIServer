using System.Threading.Tasks;
using APIServer.DTO.Auth;
using APIServer.Repository.Interfaces;
using APIServer.Services;
using APIServer.Servicies;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    readonly IMemoryDb _memoryDb;
    readonly ILogger<Login> _logger;
    readonly IAuthService _authService;
    readonly IGameService _gameService;
    readonly IDataLoadService _dataLoadService;

    public Login(ILogger<Login> logger, IMemoryDb memoryDb, IAuthService authService, IGameService gameService, IDataLoadService dataLoadService)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _authService = authService;
        _gameService = gameService;
        _dataLoadService = dataLoadService;
    }

    /// <summary>
    /// 로그인 API </br>
    /// 하이브 토큰을 검증하고, 유저가 존재하면 토큰 발급 및 저장, 로그인 시간 업데이트를 합니다.
    /// </summary>
    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequest request)
    {
        //TODO 최흥배: 큰 기능별로 함수로 나누는 것이 코드 가독성으로 좋겠습니다

        LoginResponse response = new();
        var hiveTokenValid = await _authService.VerifyTokenToHive(request.PlayerId, request.HiveToken);

        if (!hiveTokenValid)
        {
            response.Result = ErrorCode.Hive_Fail_InvalidResponse;
            return response;
        }

        //유저가 있는지 확인
        (var errorCode, var uid) = await _authService.VerifyUser(request.PlayerId);
        //없다면 생성
        if(errorCode == ErrorCode.LoginFailUserNotExist)
        {
            //TODO 최흥배: 함수 이름이 적절하지 않은 것 같습니다. 계정이 만들어지는 것이 아니고 게임DB에 유저 데이터가 만들어지는 것 아닌가요?
            (errorCode, uid) = await _authService.CreateAccountAsync(request.PlayerId, request.Nickname);
            if (errorCode != ErrorCode.None)
            {
                response.Result = errorCode;
                return response;
            }

            //TODO 최흥배: CreateAccountAsync와 같이 만들어지는 것이 더 좋을 것 같습니다. 궂이 이것과 분리할 필요가 있을까요
            // 같은 트랜잭션에 있으면 CreateAccountAsync 에 대한 롤백도 별도로 필요 없겠죠
            
            // 게임 데이터 생성 및 실패시 롤백
            errorCode = await _gameService.InitNewUserGameData(uid);
            // 실패시 앞서 만든 계정 다시 삭제.
            if (errorCode != ErrorCode.None)
            {
                await _authService.DeleteAccountAsync(uid);
                response.Result = errorCode;
                return response;
            }
        }
        else if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        response.Uid = uid;

        //토큰 발급
        var token = Security.CreateAuthToken();
        errorCode = await _memoryDb.RegistUserAsync(token, uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        
        //로그인 시간 업데이트
        errorCode = await _authService.UpdateLastLoginTime(uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        //유저 데이터 로드
        (errorCode, response.userData) = await _dataLoadService.LoadUserData(uid);

        _logger.ZLogInformation($"[Login] Uid : {uid}, Token : {token}, PlayerId : {request.PlayerId}");

        response.Token = token;
        return response;
    }
}
