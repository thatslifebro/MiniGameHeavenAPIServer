using System.Threading.Tasks;
using APIServer.Model.DTO.Auth;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class CreateAccount : ControllerBase
{
    readonly IAccountDb _accountDb;
    readonly ILogger<CreateAccount> _logger;
    readonly IAuthService _authService;


    public CreateAccount(ILogger<CreateAccount> logger, IAccountDb accountDb, IAuthService authService)
    {
        _logger = logger;
        _accountDb = accountDb;
        _authService = authService;
    }

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

        ErrorCode errorCode = await _authService.CreateAccountAsync(request.PlayerId, request.NickName);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        _logger.ZLogInformation(EventIdDic[EventType.CreateAccount], $"PlayerId : {request.PlayerId}");

        return response;

    }
}
