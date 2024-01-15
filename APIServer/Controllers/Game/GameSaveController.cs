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
public class GameSave : ControllerBase
{
    readonly ILogger<GameSave> _logger;
    readonly IGameService _gameService;

    public GameSave(ILogger<GameSave> logger, IGameService gameService)
    {
        _logger = logger;
        _gameService = gameService;
    }

    /// <summary>
    /// 게임 플레이 정보 저장 API
    /// 게임 플레이 정보(아이템 사용량, 점수, 획득 보상 등)를 저장합니다.
    /// </summary>
    [HttpPost]
    public async Task<GameSaveResponse> SaveGame(GameSaveRequest request)
    {
     GameSaveResponse response = new();
    response.Result = await _gameService.SaveGame(request.Uid, request.GameId, request.Score);
    
    _logger.ZLogInformation(EventIdDic[EventType.GameSave], $"Uid : {request.Uid}, GameId : {request.GameId}, Score : {request.Score}");
        return response;
    }
}