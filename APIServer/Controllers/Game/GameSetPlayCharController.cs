using APIServer.DTO;
using APIServer.DTO.Game;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Game;

[ApiController]
[Route("[controller]")]
public class GameSetPlayChar : ControllerBase
{
    readonly ILogger<GameSetPlayChar> _logger;
    readonly IGameService _gameService;

    public GameSetPlayChar(ILogger<GameSetPlayChar> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 게임 플레이 캐릭터 설정 API
    /// 게임 플레이에 사용할 캐릭터를 설정합니다.
    /// </summary>
    [HttpPost]
    public async Task<GameSetPlayCharResponse> SetGamePlayChar([FromHeader] HeaderDTO header, GameSetPlayCharRequest request)
    {
        GameSetPlayCharResponse response = new();

        response.Result = await _gameService.SetGamePlayChar(header.Uid, request.GameKey, request.CharKey);

        _logger.ZLogInformation($"[GameSetPlayChar] Uid : {header.Uid}, CharKey : {request.CharKey}");
        return response;
    }

}
