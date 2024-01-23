﻿using APIServer.DTO.Ranking;
using APIServer.Models;
using APIServer.Repository.Interfaces;
using APIServer.Services;
using CloudStructures;
using CloudStructures.Structures;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Repository;

public class MemoryDb : IMemoryDb
{
    readonly RedisConnection _redisConn;
    readonly ILogger<MemoryDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;
    readonly IGameDb _gameDb;

    public MemoryDb(ILogger<MemoryDb> logger, IOptions<DbConfig> dbConfig, IGameDb gameDb)
    {
        _logger = logger;
        _dbConfig = dbConfig;
        RedisConfig config = new ("default", _dbConfig.Value.Redis);
        _redisConn = new RedisConnection(config);
        _gameDb = gameDb;
    }

    public async Task<ErrorCode> RegistUserAsync(string token, int uid)
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(uid.ToString());
        ErrorCode result = ErrorCode.None;

        RdbAuthUserData user = new()
        {
            Uid = uid,
            Token = token
        };

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
            if (await redis.SetAsync(user, LoginTimeSpan()) == false)
            {
                _logger.ZLogError($"[RegistUserAsync] Uid:{uid}, Token:{token},ErrorMessage:UserBasicAuth, RedisString set Error");
                result = ErrorCode.LoginFailAddRedis;
                return result;
            }
        }
        catch
        {
            _logger.ZLogError($"[RegistUserAsync] Uid:{uid}, Token:{token},ErrorMessage:Redis Connection Error");
            result = ErrorCode.LoginFailAddRedis;
            return result;
        }

        return result;
    }

    public async Task<ErrorCode> CheckUserAuthAsync(string id, string token)
    {
        var key = MemoryDbKeyMaker.MakeUIDKey(id);
        ErrorCode result = ErrorCode.None;

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();

            if (!user.HasValue)
            {
                _logger.ZLogError( $"[CheckUserAuthAsync] Email = {id}, AuthToken = {token}, ErrorMessage:ID does Not Exist");
                result = ErrorCode.CheckAuthFailNotExist;
                return result;
            }

            if (user.Value.Uid.ToString() != id || user.Value.Token != token)
            {
                _logger.ZLogError($"[CheckUserAuthAsync] Email = {id}, AuthToken = {token}, ErrorMessage = Wrong ID or Auth Token");
                result = ErrorCode.CheckAuthFailNotMatch;
                return result;
            }
        }
        catch
        {
            _logger.ZLogError($"[CheckUserAuthAsync] Email = {id}, AuthToken = {token}, ErrorMessage:Redis Connection Error");
            result = ErrorCode.CheckAuthFailException;
            return result;
        }


        return result;
    }

    public async Task<(bool, RdbAuthUserData)> GetUserAsync(string id)
    {
        var uid = MemoryDbKeyMaker.MakeUIDKey(id);

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, uid, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();
            if (!user.HasValue)
            {
                _logger.ZLogError(
                    $"[GetUserAsync] UID = {uid}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (false, null);
            }

            return (true, user.Value);
        }
        catch
        {
            _logger.ZLogError($"[GetUserAsync] UID:{uid},ErrorMessage:ID does Not Exist");
            return (false, null);
        }
    }

    public async Task<bool> SetUserReqLockAsync(string key)
    {
        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, NxKeyTimeSpan());
            if (await redis.SetAsync(new RdbAuthUserData
            {
                // emtpy value
            }, NxKeyTimeSpan(), StackExchange.Redis.When.NotExists) == false)
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    public async Task<bool> DelUserReqLockAsync(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return false;
        }

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            var redisResult = await redis.DeleteAsync();
            return redisResult;
        }
        catch
        {
            return false;
        }
    }

    public async Task<ErrorCode> DelUserAuthAsync(int uid)
    {
        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, MemoryDbKeyMaker.MakeUIDKey(uid.ToString()), null);
            await redis.DeleteAsync();
            return ErrorCode.None;
        }
        catch
        {
            _logger.ZLogError(
                   $"[DelUserAuthAsync] UID = {uid}, ErrorCode : {ErrorCode.LogoutRedisDelFailException}");
            return ErrorCode.LogoutRedisDelFailException;
        }
    }

    public TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromMinutes(RediskeyExpireTime.LoginKeyExpireMin);
    }

    public TimeSpan TicketKeyTimeSpan()
    {
        return TimeSpan.FromSeconds(RediskeyExpireTime.TicketKeyExpireSecond);
    }

    public TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromSeconds(RediskeyExpireTime.NxKeyExpireSecond);
    }

    public async Task<ErrorCode> SetUserScore(int uid, int score)
    {
        try
        {
            var set = new RedisSortedSet<int>(_redisConn, "user-ranking", null);
            await set.AddAsync(uid, score);

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                   $"[SetUserScore] UID = {uid}, ErrorCode : {ErrorCode.SetUserScoreFailException}");
            return ErrorCode.SetUserScoreFailException;
        }
    }

    public async Task<ErrorCode> LoadUserScore()
    {
        var usersScore = await _gameDb.SelectAllUserScore();
        foreach (var userScore in usersScore)
        {
            await SetUserScore(userScore.uid, userScore.total_bestscore);
        }

        return ErrorCode.None;
    }

    public async Task<(ErrorCode, List<RankData>)> GetTopRanking()
    {
        try
        {
            List<RankData> ranking = new();

            var set = new RedisSortedSet<int>(_redisConn, "user-ranking", null);
            var rankDatas =  await set.RangeByRankWithScoresAsync(0,100,order:StackExchange.Redis.Order.Descending);

            var rank = 0;
            var score = -1;
            var count = 1;
            foreach (var rankData in rankDatas)
            {
                if(rankData.Score == score)
                {
                    count++;
                }
                else
                {
                    rank += count;
                    count = 1;
                    score = (int)rankData.Score;
                }
                ranking.Add(new RankData
                {
                    rank = rank,
                    uid = rankData.Value,
                    score = score
                });
            }

            return (ErrorCode.None, ranking);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                   $"[GetUserRanking] ErrorCode : {ErrorCode.GetRankingFailException}");
            return (ErrorCode.GetRankingFailException, null);
        }
    }

    public async Task<(ErrorCode, Int64)> GetUserRankAsync(int uid)
    {
        try
        {
            var set = new RedisSortedSet<int>(_redisConn, "user-ranking", null);
            var rank = await set.RankAsync(uid, order: StackExchange.Redis.Order.Descending);
            return (ErrorCode.None, rank.Value + 1);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                                  $"[GetUserRankAsync] UID = {uid}, ErrorCode : {ErrorCode.GetUserRankFailException}");
            return (ErrorCode.GetUserRankFailException, 0);
        }
    }

}