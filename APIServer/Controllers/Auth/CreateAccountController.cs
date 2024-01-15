using System.Threading.Tasks;
using APIServer.Model.DTO.Auth;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class CreateAccount : ControllerBase
{
    readonly IAccountDb _accountDb;
    readonly ILogger<CreateAccount> _logger;
    readonly IAuthService _authService;
    readonly IGameService _gameService;


    public CreateAccount(ILogger<CreateAccount> logger, IAccountDb accountDb, IAuthService authService, IGameService gameService)
    {
        _logger = logger;
        _accountDb = accountDb;
        _authService = authService;
        _gameService = gameService;
    }

    /// <summary>
    /// 계정 생성 API </br>
    /// 하이브 토큰을 검증하고, 계정이 없다면 계정을 생성합니다.
    /// 초기 게임 데이터를 생성합니다.
    /// </summary>
    [HttpPost]
    public async Task<CreateAccountRes> Post(CreateAccountReq request)
    {
        var response = new CreateAccountRes();
        var tokenValid = await _authService.VerifyTokenToHive(request.PlayerId, request.HiveToken);
        if (!tokenValid)
        {
            response.Result = ErrorCode.Hive_Fail_InvalidResponse;
            return response;
        }

        (var errorCode, var uid) = await _authService.CreateAccountAsync(request.PlayerId, request.NickName);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        errorCode = await _gameService.InitNewUserGameData(uid);
        if (errorCode != ErrorCode.None)
        {
            // 게임 데이터 생성 실패시 앞서 만든 계정 다시 삭제.
            await _authService.DeleteAccountAsync(uid);
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[CreateAccount] PlayerId : {request.PlayerId}");

        return response;

    }
}
