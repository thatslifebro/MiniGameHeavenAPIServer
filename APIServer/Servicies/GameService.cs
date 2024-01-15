using APIServer.Controllers.Friend;
using APIServer.Controllers.Game;
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

    public async Task<(ErrorCode, IEnumerable<GdbGameListInfo>)> GetGameList(int uid)
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

    public async Task<ErrorCode> UnlockGame(int uid, int gameId)
    {
        try
        {
            var rowCount = await _gameDb.InsertGame(uid, gameId);
            if(rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Game.SetNewUserGameList] ErrorCode: {ErrorCode.GameUnlockFailInsert}, Uid: {uid}");
                return ErrorCode.GameUnlockFailInsert;
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.GameUnlock] ErrorCode: {ErrorCode.GameUnlockFailException}, Uid: {uid}");
            return ErrorCode.GameUnlockFailException;
        }
    }

    public async Task<(ErrorCode, GdbGameInfo)> GetGameInfo(int uid, int gameId)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetGameInfo(uid,gameId));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.GetGameList] ErrorCode: {ErrorCode.GameInfoFailException}, Uid: {uid}");
            return (ErrorCode.GameInfoFailException, null);
        }
    }

    public async Task<ErrorCode> SaveGame(int uid, int gameId, int score)
    {
        try
        {
            var row = await _gameDb.UpdateBestscore(uid, gameId, score);
            if(row == 0)
            {
                row = await _gameDb.UpdateBestscoreCurSeason(uid, gameId, score);
                if(row == 0)
                {
                    await _gameDb.UpdateRecentPlayDt(uid, gameId);
                }
            }
            
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Game.GameSave] ErrorCode: {ErrorCode.GameSaveFailException}, Uid: {uid}");
            return ErrorCode.GameSaveFailException;
        }
    }

    public async Task<ErrorCode> InitNewUserGameData(int uid)
    {
        var transaction = _gameDb.GDbConnection().BeginTransaction();
        try
        {
            var rowCount = await _gameDb.InsertInitGameList(uid, transaction);
            if (rowCount != 3)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailGameList;
            }
            rowCount = await _gameDb.InsertInitCharacter(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailCharacter;
            }
            transaction.Commit();
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.InitNewUserGameData] ErrorCode: {ErrorCode.InitNewUserGameDataFailException}, Uid: {uid}");
            return ErrorCode.GameSetNewUserListFailException;
        }
        finally
        {
            transaction.Dispose();
        }
    }
}
