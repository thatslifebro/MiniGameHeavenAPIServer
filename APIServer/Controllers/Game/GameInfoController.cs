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
public class GameInfo : ControllerBase
{
    readonly ILogger<GameInfo> _logger;
    readonly IGameService _gameService;

    public GameInfo(ILogger<GameInfo> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 게임 정보 조회 API
    /// 게임의 정보(아이템 보유 현황, 플레이 캐릭터(코스튬,스킨), 최고점수 등)을 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<GameInfoResponse> GetGameInfo([FromHeader] HeaderDTO header, GameInfoRequest request)
    {
    GameInfoResponse response = new();

    (response.Result, response.GameInfo) = await _gameService.GetGameInfo(header.Uid, request.GameKey);
    
    _logger.ZLogInformation($"[GameInfo] Uid : {header.Uid}, GameId : {request.GameKey}");
        return response;
    }
}
