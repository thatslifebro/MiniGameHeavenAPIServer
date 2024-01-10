using System;
using System.Threading.Tasks;
using APIServer.Model.DTO;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class CreateAccount : ControllerBase
{
    readonly IAccountDb _accountDb;
    readonly ILogger<CreateAccount> _logger;
    readonly IVerifyTokenService _verifyTokenService;


    public CreateAccount(ILogger<CreateAccount> logger, IAccountDb accountDb, IVerifyTokenService verifyTokenService)
    {
        _logger = logger;
        _accountDb = accountDb;
        _verifyTokenService = verifyTokenService;
    }

    [HttpPost]
    public async Task<CreateAccountRes> Post(CreateAccountReq request)
    {
        var response = new CreateAccountRes();
        var tokenValid = await _verifyTokenService.VerifyTokenToHive(request.PlayerId, request.Token);

        if (!tokenValid)
        {
            response.Result = ErrorCode.Hive_Fail_InvalidResponse;
            return response;
        }

        response.Result = await _accountDb.CreateAccountAsync(request.PlayerId, request.NickName);

        return response;

    }
}
