using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;
using APIServer.DTO;
using APIServer.DTO.Game;

namespace APIServer.Controllers.Game;

[ApiController]
[Route("[controller]")]
public class GameUnlock : ControllerBase
{
    readonly ILogger<GameUnlock> _logger;
    readonly IGameService _gameService;

    public GameUnlock(ILogger<GameUnlock> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    ///<summary>
    /// 게임 잠금 해제 API
    /// 게임을 잠금 해제합니다.
    /// </summary>
    [HttpPost]
    public async Task<GameUnlockResponse> UnlockGame([FromHeader] HeaderDTO header, GameUnlockRequest request)
    {
    GameUnlockResponse response = new();

    response.Result = await _gameService.UnlockGame(header.Uid, request.GameKey);
    
    _logger.ZLogInformation($"[GameUnlock] Uid : {header.Uid}, GameId : {request.GameKey}");
        return response;
    }
}
