// 클라이언트가 플랫폼 서버에서 인증을 받았는지 확인해 준다
// 클라이언트는 플랫폼 서버에서 받은 인증토큰과 자신의 인증ID(계정 ID 혹은 플랫폼에서 만들어준 ID)로 보낸다.
// 사용할 수 있는 인증ID와 인증토큰은 이미 정해져 있다.
// 게임 서버는 인증이 성공하면
// - 이 유저의 default 게임데이터가 없다면 생성해줘야 한다.

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
        Tuple<ErrorCode, Int64> output;

        output = await _hiveDb.VerifyUser(request.Email, request.Password);

        string token = JWT.IssueToken(output.Item2);

        response.Result = output.Item1;
        response.PlayerId = output.Item2;
        response.Token = token;

        return response;

    }
}

