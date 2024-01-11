using System.Threading.Tasks;
using APIServer.Model.DTO.Auth;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class Logout : ControllerBase
{
    readonly IMemoryDb _memoryDb;
    readonly ILogger<Logout> _logger;

    public Logout(ILogger<Logout> logger, IAccountDb accountDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    [HttpDelete]
    public async Task<LogoutResponse> Post(LogoutRequest request)
    {
        LogoutResponse response = new();
        var errorCode = await _memoryDb.DelUserAuthAsync(request.Uid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }
        _logger.ZLogInformation(EventIdDic[EventType.CreateAccount], $"Uid : {request.Uid}");

        return response;
    }
}
