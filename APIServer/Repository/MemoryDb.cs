﻿using APIServer.Model.DAO;
using APIServer.Services;
using CloudStructures;
using CloudStructures.Structures;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;
using static LogManager;

namespace APIServer.Repository;

public class MemoryDb : IMemoryDb
{
    public RedisConnection _redisConn;
    private static readonly ILogger<MemoryDb> s_logger = GetLogger<MemoryDb>();

    public void Init(string address)
    {
        RedisConfig config = new("default", address);
        _redisConn = new RedisConnection(config);

        s_logger.ZLogDebug($"userDbAddress:{address}");
    }


    public async Task<ErrorCode> RegistUserAsync(string token, int uid)
    {
        string key = MemoryDbKeyMaker.MakeUIDKey(uid.ToString());
        ErrorCode result = ErrorCode.None;

        RdbAuthUserData user = new()
        {
            Uid = uid,
            Token = token,
            State = UserState.Default.ToString()
        };

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, LoginTimeSpan());
            if (await redis.SetAsync(user, LoginTimeSpan()) == false)
            {
                s_logger.ZLogError(EventIdDic[EventType.LoginAddRedis],
                    $"Uid:{uid}, Token:{token},ErrorMessage:UserBasicAuth, RedisString set Error");
                result = ErrorCode.LoginFailAddRedis;
                return result;
            }
        }
        catch
        {
            s_logger.ZLogError(EventIdDic[EventType.LoginAddRedis],
                $"Uid:{uid}, Token:{token},ErrorMessage:Redis Connection Error");
            result = ErrorCode.LoginFailAddRedis;
            return result;
        }

        return result;
    }

    public async Task<ErrorCode> CheckUserAuthAsync(string id, string token)
    {
        string key = MemoryDbKeyMaker.MakeUIDKey(id);
        ErrorCode result = ErrorCode.None;

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();

            if (!user.HasValue)
            {
                s_logger.ZLogError(EventIdDic[EventType.Login],
                    $"RedisDb.CheckUserAuthAsync: Email = {id}, AuthToken = {token}, ErrorMessage:ID does Not Exist");
                result = ErrorCode.CheckAuthFailNotExist;
                return result;
            }

            if (user.Value.Uid.ToString() != id || user.Value.Token != token)
            {
                s_logger.ZLogError(EventIdDic[EventType.Login],
                    $"RedisDb.CheckUserAuthAsync: Email = {id}, AuthToken = {token}, ErrorMessage = Wrong ID or Auth Token");
                result = ErrorCode.CheckAuthFailNotMatch;
                return result;
            }
        }
        catch
        {
            s_logger.ZLogError(EventIdDic[EventType.Login],
                $"RedisDb.CheckUserAuthAsync: Email = {id}, AuthToken = {token}, ErrorMessage:Redis Connection Error");
            result = ErrorCode.CheckAuthFailException;
            return result;
        }


        return result;
    }

    public async Task<bool> SetUserStateAsync(RdbAuthUserData user, UserState userState)
    {
        string key = MemoryDbKeyMaker.MakeUIDKey(user.Uid.ToString());
        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, key, null);

            user.State = userState.ToString();

            return await redis.SetAsync(user) != false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool, RdbAuthUserData)> GetUserAsync(int id)
    {
        string uid = MemoryDbKeyMaker.MakeUIDKey(id.ToString());

        try
        {
            RedisString<RdbAuthUserData> redis = new(_redisConn, uid, null);
            RedisResult<RdbAuthUserData> user = await redis.GetAsync();
            if (!user.HasValue)
            {
                s_logger.ZLogError(
                    $"RedisDb.UserStartCheckAsync: UID = {uid}, ErrorMessage = Not Assigned User, RedisString get Error");
                return (false, null);
            }

            return (true, user.Value);
        }
        catch
        {
            s_logger.ZLogError($"UID:{uid},ErrorMessage:ID does Not Exist");
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
            bool redisResult = await redis.DeleteAsync();
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
            s_logger.ZLogError(
                   $"RedisDb.DelUserAuthAsync: UID = {uid}, ErrorCode : {ErrorCode.LogoutRedisDelFailException}");
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
}