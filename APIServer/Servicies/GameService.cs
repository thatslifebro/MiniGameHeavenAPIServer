﻿using APIServer.Controllers.Friend;
using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class GameService :IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IAccountDb _accountDb;
    readonly IGameDb _gameDb;
    public GameService(ILogger<GameService> logger, IAccountDb accountDb, IGameDb gameDb)
    {
        _accountDb = accountDb;
        _logger = logger;
        _gameDb = gameDb;
    }

    public async Task<(ErrorCode, IEnumerable<GdbGameInfo>)> GetGameList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetGameList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.GetGameList] ErrorCode: {ErrorCode.GameListFailException}, Uid: {uid}");
            return (ErrorCode.GameListFailException, null);
        }
    }

    public async Task<ErrorCode> InitGameList(int uid)
    {
        try
        {
            var rowCount = await _gameDb.InsertInitGameList(uid);
            if (rowCount != 3)
            {
                _logger.ZLogDebug(
                $"[Game.SetNewUserGameList] ErrorCode: {ErrorCode.GameSetNewUserListFailInsert}, Uid: {uid}");
                return ErrorCode.GameSetNewUserListFailInsert;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.SetNewUserGameList] ErrorCode: {ErrorCode.GameSetNewUserListFailException}, Uid: {uid}");
            return ErrorCode.GameSetNewUserListFailException;
        }
    }
}
