using APIServer.DTO.Game;
using APIServer.Models;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class GameService :IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IGameDb _gameDb;
    public GameService(ILogger<GameService> logger, IGameDb gameDb)
    {
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

    public async Task<ErrorCode> UnlockGame(int uid, int gameKey)
    {
        try
        {
            var gameInfo = await _gameDb.GetGameInfo(uid, gameKey);
            if (gameInfo != null)
            {
                _logger.ZLogDebug($"[Game.GameUnlock] ErrorCode: { ErrorCode.GameUnlockFailAlreadyUnlocked}, Uid: { uid}");
                return ErrorCode.GameUnlockFailAlreadyUnlocked;
            }

            var rowCount = await _gameDb.InsertGame(uid, gameKey);
            if(rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Game.GameUnlock] ErrorCode: {ErrorCode.GameUnlockFailInsert}, Uid: {uid}");
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

    public async Task<(ErrorCode, GdbGameInfo)> GetGameInfo(int uid, int gameKey)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetGameInfo(uid,gameKey));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Game.GetGameList] ErrorCode: {ErrorCode.GameInfoFailException}, Uid: {uid}");
            return (ErrorCode.GameInfoFailException, null);
        }
    }

    public async Task<ErrorCode> SaveGame(int uid, int gameKey, int score, List<UsedFoodData> foods)
    {
        try
        {
            var gameInfo = await _gameDb.GetGameInfo(uid, gameKey);

            if (gameInfo == null)
            {
                _logger.ZLogError($"[Game.GameSave] ErrorCode: {ErrorCode.GameSaveFailGameLocked}, Uid: {uid}");
                return ErrorCode.GameSaveFailGameLocked;
            }

            //최고점수 갱신
            var rowCount = await _gameDb.UpdateBestscore(uid, gameKey, score);
            if(rowCount == 0)
            {
                rowCount = await _gameDb.UpdateBestscoreCurSeason(uid, gameKey, score);
                if(rowCount == 0)
                {
                    await _gameDb.UpdateRecentPlayDt(uid, gameKey);
                }
            }

            //사용된 푸드 감소
            foreach(var food in foods)
            {
                rowCount = await _gameDb.FoodDecrement(uid, food.FoodKey, food.FoodQty);
                if(rowCount != 1)
                {
                    _logger.ZLogError($"[Game.GameSave] ErrorCode: {ErrorCode.GameSaveFailFoodDecrement}, Uid: {uid}");
                    return ErrorCode.GameSaveFailFoodDecrement;
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
            var rowCount = await _gameDb.InsertInitCharacter(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailCharacter;
            }

            rowCount = await _gameDb.InsertInitGameList(uid, transaction);
            if (rowCount != 3)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailGameList;
            }

            rowCount = await _gameDb.InsertInitMoneyInfo(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailMoney;
            }

            rowCount = await _gameDb.InsertInitAttendance(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return ErrorCode.InitNewUserGameDataFailAttendance;
            }

            transaction.Commit();
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.ZLogError(e,
                $"[Game.InitNewUserGameData] ErrorCode: {ErrorCode.InitNewUserGameDataFailException}, Uid: {uid}");
            return ErrorCode.GameSetNewUserListFailException;
        }
        finally
        {
            transaction.Dispose();
        }
    }

    public async Task<ErrorCode> SetGamePlayChar(int uid, int gameKey, int charKey)
    {
        try
        {
            var charInfo = await _gameDb.GetCharInfo(uid, charKey);
            if(charInfo == null)
            {
                return ErrorCode.CharNotExist;
            }

            var rowCount = await _gameDb.UpdateGamePlayChar(uid, gameKey, charKey);
            if (rowCount != 1)
            {
                _logger.ZLogError($"[Game.SetPlayChar] ErrorCode: {ErrorCode.GameSetPlayCharFailUpdate}, Uid: {uid}");
                return ErrorCode.GameSetPlayCharFailUpdate;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Game.SetPlayChar] ErrorCode: {ErrorCode.GameSetPlayCharFailException}, Uid: {uid}");
            return ErrorCode.GameSetPlayCharFailException;
        }
    }
}
