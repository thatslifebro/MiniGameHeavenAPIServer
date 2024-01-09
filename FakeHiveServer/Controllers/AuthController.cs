using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using APIServer.Model.DTO;
using APIServer.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace APIServer.Controllers;

[ApiController]
[Route("[controller]")]
public class Auth : ControllerBase
{
    readonly ILogger<Auth> _logger;
    readonly IHiveDb _hiveDb;
    public Auth(ILogger<Auth> logger, IHiveDb hiveDb)
    {
        _logger = logger;
        _hiveDb = hiveDb;
    }

    [HttpPost]
    public async Task<CreateHiveAccountResponse> CreateHiveAccount([FromBody]CreateHiveAccountRequest request)
    {
        CreateHiveAccountResponse response = new();

        response.Result = await _hiveDb.CreateAccountAsync(request.Email, request.Password);

        return response;
    }

    [HttpPost]
    [Route("login")]

    public async Task<LoginHiveResponse> LoginHive([FromBody]LoginHiveRequest request)
    {
        LoginHiveResponse response = new();
        Tuple<ErrorCode, Int64> output = await _hiveDb.VerifyUser(request.Email, request.Password);

        string token = JWT.IssueToken(output.Item2);

        response.Result = output.Item1;
        response.PlayerId = output.Item2;
        response.Token = token;

        return response;

    }
}

