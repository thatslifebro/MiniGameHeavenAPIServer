using APIServer.DTO.Game;
using APIServer.Models;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class GameService :IGameService
{
    readonly ILogger<GameService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    int initCharKey;

    public GameService(ILogger<GameService> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        initCharKey = _masterDb._characterList[0].char_key;
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

            var rowCount = await _gameDb.InsertGame(uid, initCharKey, gameKey);
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
            return (ErrorCode.None, await _gameDb.GetGameInfo(uid, gameKey));
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

            //점수 업데이트
            //게임 최고점수 갱신
            var rowCount = await _gameDb.UpdateBestscore(uid, gameKey, score);
            if(rowCount == 0)
            {
                //게임 시즌 최고 점수 갱신
                rowCount = await _gameDb.UpdateBestscoreCurSeason(uid, gameKey, score);
                if(rowCount == 0)
                {
                    // 최고 점수 갱신이 없을 때
                    await _gameDb.UpdateRecentPlayDt(uid, gameKey);
                }
            }

            //유저 최고점수 갱신
            await _gameDb.UpdateUserBestScoreEver(uid);

            //사용된 푸드 감소
            if(foods != null)
            {
                foreach (var food in foods)
                {
                    rowCount = await _gameDb.FoodDecrement(uid, food.FoodKey, food.FoodQty);
                    if (rowCount != 1)
                    {
                        _logger.ZLogError($"[Game.GameSave] ErrorCode: {ErrorCode.GameSaveFailFoodDecrement}, Uid: {uid}");
                        return ErrorCode.GameSaveFailFoodDecrement;
                    }
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

    public async Task<(ErrorCode, int)> InitNewUserGameData(Int64 playerId, string nickname)
    {
        var transaction = _gameDb.GDbConnection().BeginTransaction();
        try
        {
            var (errorCode, uid) = await CreateUserAsync(playerId, nickname, transaction);
            if(errorCode != ErrorCode.None)
            {
                transaction.Rollback();
                return (errorCode,0);
            }

            var rowCount = await _gameDb.InsertInitCharacter(uid, initCharKey, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailCharacter, 0);
            }

            rowCount = await _gameDb.InsertInitGameList(uid, initCharKey, transaction);
            if (rowCount != 3)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailGameList, 0);
            }

            rowCount = await _gameDb.InsertInitMoneyInfo(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailMoney, 0);
            }

            rowCount = await _gameDb.InsertInitAttendance(uid, transaction);
            if (rowCount != 1)
            {
                transaction.Rollback();
                return (ErrorCode.InitNewUserGameDataFailAttendance, 0);
            }

            transaction.Commit();
            return (ErrorCode.None, uid);
        }
        catch (Exception e)
        {
            transaction.Rollback();
            _logger.ZLogError(e,
                $"[Game.InitNewUserGameData] ErrorCode: {ErrorCode.InitNewUserGameDataFailException}, PlayerId : {playerId}");
            return (ErrorCode.GameSetNewUserListFailException, 0);
        }
        finally
        {
            transaction.Dispose();
        }
    }

    async Task<(ErrorCode,int)> CreateUserAsync(Int64 playerId, string nickname, IDbTransaction transaction)
    {
        try
        {
            if (string.IsNullOrEmpty(nickname))
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailNoNickname}, nickname : {nickname}");
                return (ErrorCode.CreateUserFailNoNickname,0);
            }
            //nickname 중복 체크
            var existUser = await _gameDb.GetUserByNickname(nickname, transaction);
            if (existUser is not null)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailDuplicateNickname}, nickname : {nickname}");
                return (ErrorCode.CreateUserFailDuplicateNickname,0);
            }

            //유저 생성
            return (ErrorCode.None, await _gameDb.InsertUser(playerId, nickname, transaction));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[CreateAccount] ErrorCode: {ErrorCode.CreateUserFailException}, PlayerId: {playerId}");
            return (ErrorCode.CreateUserFailException, 0);
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
