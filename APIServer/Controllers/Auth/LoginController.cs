using System.Threading.Tasks;
using APIServer.Model.DTO.Auth;
using APIServer.Repository;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    readonly IMemoryDb _memoryDb;
    readonly ILogger<Login> _logger;
    readonly IAuthService _authService;

    public Login(ILogger<Login> logger, IAccountDb accountDb, IMemoryDb memoryDb, IAuthService authService)
    {
        _logger = logger;
        _memoryDb = memoryDb;
        _authService = authService;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequest request)
    {
        LoginResponse response = new();
        var hiveTokenValid = await _authService.VerifyTokenToHive(request.PlayerId, request.HiveToken);

        if (!hiveTokenValid)
        {
            response.Result = ErrorCode.Hive_Fail_InvalidResponse;
            return response;
        }

        (var errorCode, var uid) = await _authService.VerifyUser(request.PlayerId);
        response.Uid = uid;
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        var token = Security.CreateAuthToken();
        errorCode = await _memoryDb.RegistUserAsync(token, uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        errorCode = await _authService.UpdateLastLoginTime(uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation(EventIdDic[EventType.Login], $"Uid : {uid}, Token : {token}, PlayerId : {request.PlayerId}");

        response.Token = token;
        return response;
    }
}
