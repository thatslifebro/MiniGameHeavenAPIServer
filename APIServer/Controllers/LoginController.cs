using System;
using System.Threading.Tasks;
using APIServer.Model.DTO;
using APIServer.Repository;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Login : ControllerBase
{
    private readonly IAccountDb _accountDb;
    private readonly IMemoryDb _memoryDb;
    private readonly ILogger<Login> _logger;
    readonly IVerifyTokenService _verifyTokenService;

    public Login(ILogger<Login> logger, IAccountDb accountDb, IMemoryDb memoryDb, IVerifyTokenService verifyTokenService)
    {
        _logger = logger;
        _accountDb = accountDb;
        _memoryDb = memoryDb;
        _verifyTokenService = verifyTokenService;
    }

    [HttpPost]
    public async Task<LoginResponse> Post(LoginRequest request)
    {
        LoginResponse response = new();
        var hiveTokenValid = await _verifyTokenService.VerifyTokenToHive(request.PlayerId, request.HiveToken);

        if (!hiveTokenValid)
        {
            response.Result = ErrorCode.Hive_Fail_InvalidResponse;
            return response;
        }

        // ID, PW 검증
        (ErrorCode errorCode, int uid) = await _accountDb.VerifyUser(request.PlayerId);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        string token = Security.CreateAuthToken();
        errorCode = await _memoryDb.RegistUserAsync(token, uid);
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
