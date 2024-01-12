using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LogManager;
using ZLogger;
using APIServer.Model.DTO.Friend;

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
    /// 보유 게임 정보 조회 API
    /// 보유한 게임의 목록과 정보를 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<GameInfoResponse> GetGameInfo(GameInfoRequest request)
    {
    GameInfoResponse response = new();
    (response.Result, response.GameInfo) = await _gameService.GetGameInfo(request.Uid, request.GameId);
    
    _logger.ZLogInformation(EventIdDic[EventType.GameInfo], $"Uid : {request.Uid}, GameId : {request.GameId}");
        return response;
    }
}
