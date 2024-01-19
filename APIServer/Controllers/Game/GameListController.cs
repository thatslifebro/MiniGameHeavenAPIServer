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
public class GameList : ControllerBase
{
    readonly ILogger<GameList> _logger;
    readonly IGameService _gameService;

    public GameList(ILogger<GameList> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 보유 게임 정보 조회 API
    /// 보유한 게임의 목록과 정보를 조회합니다.
    /// </summary>
    [HttpGet]
    public async Task<GameListResponse> GetGameList([FromHeader] HeaderDTO header)
    {
        GameListResponse response = new();

        (response.Result, response.GameList) = await _gameService.GetGameList(header.Uid);

        _logger.ZLogInformation($"[GameList] Uid : {header.Uid}");
        return response;
    }
}
