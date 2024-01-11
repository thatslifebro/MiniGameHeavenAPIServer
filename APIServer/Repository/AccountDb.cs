using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APIServer.Controllers.Friend;
using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
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

    public async Task<AdbUserInfo> GetUserByNickname(string nickname)
    {
        return await _queryFactory.Query("user_info")
                                .Where("nickname", nickname)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<int> InsertUser(Int64 playerId, string nickname)
    {
        return await _queryFactory.Query("user_info")
                                .InsertGetIdAsync<int>(new
                                {
                                    player_id = playerId,
                                    nickname = nickname,
                                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                });
    }

    public async Task<int> UpdateRecentLogin(int uid)
    {
        return await _queryFactory.Query("user_info").Where("uid", uid).UpdateAsync(new
        {
            recent_login_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
        });
    }

    

    public async Task<ErrorCode> AddFriendByUid(int uid, int friendUid)
    {
        try
        {
            AdbUserInfo userInfo = await _queryFactory.Query("user_info")
                                    .Where("uid", friendUid)
                                    .FirstOrDefaultAsync<AdbUserInfo>();
            //없는 유저라면 에러
            if (userInfo is null)
            {
                return ErrorCode.FriendAddFailUserNotExist;
            }

            //이미 친구신청 했다면 에러
            AdbFriendInfo frinedInfo = await _queryFactory.Query("friend")
                                    .Where("uid", uid)
                                    .Where("friend_uid", friendUid)
                                    .FirstOrDefaultAsync<AdbFriendInfo>();
            if (frinedInfo is not null)
            {
                return ErrorCode.FriendAddFailAlreadyFriend;
            }

            //상대방이 친구신청을 하지 않았을 때
            AdbFriendInfo frinedAddMeInfo = await _queryFactory.Query("friend")
                                    .Where("uid", friendUid)
                                    .Where("friend_uid", uid)
                                    .FirstOrDefaultAsync<AdbFriendInfo>();
            int count = 0;
            if (frinedAddMeInfo is null)
            {
                count = await _queryFactory.Query("friend").InsertAsync(new
                {
                    uid = uid,
                    friend_uid = friendUid,
                    create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                });
                return count != 1 ? ErrorCode.FriendAddFailInsert : ErrorCode.None;
            }

            //상대방이 친구신청를 했다면 accept_yn을 true로 하여 친구등록
            var transaction = _dbConn.BeginTransaction();

            count = await _queryFactory.Query("friend").InsertAsync(new
            {
                uid = uid,
                friend_uid = friendUid,
                create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                accept_yn = true,
            },transaction);
            if (count != 1)
            {
                transaction.Rollback();
                return ErrorCode.FriendAddFailInsert;
            }
            count = await _queryFactory.Query("friend").Where("uid", friendUid).Where("friend_uid", uid).UpdateAsync(new
            {
                accept_yn = true,
            }, transaction);
            if (count != 1)
            {
                transaction.Rollback();
                return ErrorCode.FriendAddFailInsert;
            }
            transaction.Commit();
            transaction.Dispose();

            return ErrorCode.None;
        }
        catch(Exception e)
        {
            _logger.ZLogDebug(
                $"[AccountDb.Login] ErrorCode: {ErrorCode.FriendAddFailException}, Uid: {uid}, e :{e.Message}");
            return ErrorCode.FriendAddFailException;
        }

    }

    public async Task<(ErrorCode, List<FriendInfo>)> GetFriendList(int uid)
    {
        try
        {
            List<FriendInfo> friendList = (List<FriendInfo>) await _queryFactory.Query("friend")
                                                .Join("user_info", "user_info.uid", "friend.friend_uid")
                                                .Where("friend.uid", uid)
                                                .Where("accept_yn", true) 
                                                .Select("user_info.uid", "user_info.nickname")// FriendInfo에 따라 변경 필요
                                                .GetAsync<FriendInfo>();
            return (ErrorCode.None,friendList);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendGetListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetListFailException, null);
        }
    }

    public async Task<(ErrorCode, List<FriendRequestInfo>)> GetFriendReceivedReqList(int uid)
    {
        try
        {
            List<FriendRequestInfo> friendList = (List<FriendRequestInfo>)await _queryFactory.Query("friend")
                                                .Join("user_info", "user_info.uid", "friend.uid")
                                                .Where("friend.friend_uid", uid)
                                                .Where("accept_yn", false)
                                                .Select("user_info.uid", "user_info.nickname")
                                                .GetAsync<FriendRequestInfo>();
            return (ErrorCode.None, friendList);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendGetRequestListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetRequestListFailException, null);
        }
    }

    public async Task<(ErrorCode, List<FriendRequestInfo>)> GetFriendSentReqList(int uid)
    {
        try
        {
            List<FriendRequestInfo> friendList = (List<FriendRequestInfo>)await _queryFactory.Query("friend")
                                                .Join("user_info", "user_info.uid", "friend.friend_uid")
                                                .Where("friend.uid", uid)
                                                .Where("accept_yn", false)
                                                .Select("user_info.uid", "user_info.nickname")
                                                .GetAsync<FriendRequestInfo>();
            return (ErrorCode.None, friendList);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendGetRequestListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetRequestListFailException, null);
        }
    }

    public async Task<ErrorCode> DeleteFriend(int uid, int friendUid)
    {
        try
        {
            //친구가 아니라면
            AdbFriendInfo frinedInfo = await _queryFactory.Query("friend")
                                    .Where("uid", uid)
                                    .Where("friend_uid", friendUid)
                                    .Where("accept_yn", true)
                                    .FirstOrDefaultAsync<AdbFriendInfo>();
            if (frinedInfo is null)
            {
                return ErrorCode.FriendDeleteFailNotFriend;
            }

            int count = await _queryFactory.Query("friend")
                                    .WhereRaw($"(uid={uid} AND friend_uid={friendUid}) OR (uid={friendUid} AND friend_uid={uid})")
                                    .DeleteAsync();
            return count != 2 ? ErrorCode.FriendDeleteFailDelete : ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,$"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendDeleteFailException}, Uid: {uid}");
            return ErrorCode.FriendDeleteFailException;
        }
    }

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.AccountDb);

        _dbConn.Open();
    }

    private void Close()
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