using System;
using System.Threading.Tasks;
using APIServer.Model.DTO.Auth;
using APIServer.Repository;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using ZLogger;
using static LogManager;

namespace APIServer.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class Logout : ControllerBase
{
    private readonly IMemoryDb _memoryDb;
    private readonly ILogger<Logout> _logger;

    public Logout(ILogger<Logout> logger, IAccountDb accountDb, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    [HttpDelete]
    public async Task<LogoutResponse> Post(LogoutRequest request)
    {
        LogoutResponse response = new();
        response.Result = await _memoryDb.DelUserAuthAsync(request.Uid);
        return response;
    }
}
