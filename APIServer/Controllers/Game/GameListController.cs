﻿using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LogManager;
using ZLogger;
using APIServer.Model.DTO.Friend;

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
    [HttpPost]
    public async Task<GameListResponse> GetFriendList(GameListRequest request)
    {
        GameListResponse response = new();
        (response.Result, response.GameList) = await _gameService.GetGameList(request.Uid);

        _logger.ZLogInformation(EventIdDic[EventType.GameList], $"Uid : {request.Uid}");
        return response;
    }
}