using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using APIServer.Controllers.Friend;
using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata;
using SqlKata.Execution;
using ZLogger;

namespace APIServer.Repository;

public class AccountDb : IAccountDb
{
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<AccountDb> _logger;
    IDbConnection _dbConn;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;

    public AccountDb(ILogger<AccountDb> logger, IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    public async Task<AdbUserInfo> GetUserByPlayerId(Int64 playerId)
    {
        return await _queryFactory.Query("user_info")
                                .Where("player_id", playerId)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<AdbUserInfo> GetUserByUid(int uid)
    {
        return await _queryFactory.Query("user_info")
                                .Where("uid", uid)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<AdbUserInfo> GetUserByNickname(string nickname)
    {
        return await _queryFactory.Query("user_info")
                                .Where("nickname", nickname)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<int> InsertUser(Int64 playerId, string nickname)
    {
        return await _queryFactory.Query("user_info")
                                .InsertAsync(new
                                {
                                    player_id = playerId,
                                    nickname = nickname,
                                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                });
    }
    
    public IDbConnection ADbConnection()
    {
        return _queryFactory.Connection;
    } 

    public async Task<int> UpdateRecentLogin(int uid)
    {
        return await _queryFactory.Query("user_info").Where("uid", uid).UpdateAsync(new
        {
            recent_login_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
        });
    }

    public async Task<AdbFriendReqInfo> GetFriendReqInfo(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .FirstOrDefaultAsync<AdbFriendReqInfo>();
    }

    public async Task<int> InsertFriendReq(int uid, int friendUid, bool accept=false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    accept_yn = accept,
                                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                });
    }
    public async Task<int> InsertFriendReq(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    accept_yn = accept,
                                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                }, transaction);
    }

    public async Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend").Where("uid", friendUid).Where("friend_uid", uid).UpdateAsync(new
        {
            accept_yn = accept,
        }, transaction);
    }

    public async Task<IEnumerable<FriendUserInfo>> GetFriendUserInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user_info", "user_info.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", true)
                                .Select("user_info.uid", "user_info.nickname")// FriendUserInfo에 따라 변경 필요
                                .GetAsync<FriendUserInfo>();
    }

    public async Task<IEnumerable<FriendReqInfo>> GetFriendReceivedReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user_info", "user_info.uid", "friend.uid")
                                .Where("friend.friend_uid", uid)
                                .Where("accept_yn", false)
                                .Select("user_info.uid", "user_info.nickname")
                                .GetAsync<FriendReqInfo>();
    }

    public async Task<IEnumerable<FriendReqInfo>> GetFriendSentReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user_info", "user_info.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", false)
                                .Select("user_info.uid", "user_info.nickname")
                                .GetAsync<FriendReqInfo>();
    }

    public async Task<int> DeleteFriendEachOther(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .WhereRaw($"(uid={uid} AND friend_uid={friendUid}) OR (uid={friendUid} AND friend_uid={uid})")
                                .DeleteAsync();
    }

    public async Task<int> DeleteFriendReq(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid",uid)
                                .Where("friend_uid", friendUid)
                                .DeleteAsync();
    }

    void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.AccountDb);

        _dbConn.Open();
    }

    void Close()
    {
        _dbConn.Close();
    }
}

public class DbConfig
{
    public string MasterDb { get; set; }
    public string AccountDb { get; set; }
    public string GameDb { get; set; }
    public string Memcached { get; set; }
}