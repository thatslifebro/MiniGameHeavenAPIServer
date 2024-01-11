using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APIServer.Model.DAO;
using APIServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
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

    public async Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname)
    {
        try
        {
            //playerId 중복 체크
            var existPlayerId = await _queryFactory.Query("user_info").Where("player_id", playerId).GetAsync<Int64>();
            if (existPlayerId.Count()!=0)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountAlreadyExistFail}, PlayerId : {playerId}");
                return ErrorCode.CreateAccountDuplicateFail;
            }
            //nickname 중복 체크
            var existNickname = await _queryFactory.Query("user_info").Where("nickname", nickname).GetAsync<string>();
            if (existNickname.Count() != 0)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountDuplicateFail}, nickname : {nickname}");
                return ErrorCode.CreateAccountDuplicateFail;
            }
            //account 생성
            int count = await _queryFactory.Query("user_info").InsertAsync(new
            {
                uid = 0,
                player_id = playerId,
                nickname = nickname,
                create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            });

            _logger.ZLogDebug($"[CreateAccount] PlayerId: {playerId}");

            return count != 1 ? ErrorCode.CreateAccountFailInsert : ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[AccountDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, PlayerId: {playerId}");
            return ErrorCode.CreateAccountFailException;
        }
    }

    public async Task<(ErrorCode, int)> VerifyUser(Int64 playerId)
    {
        try
        {
            //playerId로 userInfo 조회
            AdbUserInfo userInfo = await _queryFactory.Query("user_info")
                                    .Where("player_id", playerId)
                                    .FirstOrDefaultAsync<AdbUserInfo>();
            //없는 유저라면 에러
            if (userInfo is null)
            {
                return (ErrorCode.LoginFailUserNotExist, 0);
            }

            return (ErrorCode.None, userInfo.uid);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[AccountDb.VerifyAccount] ErrorCode: {ErrorCode.LoginFailException}, PlayerId: {playerId}");
            return (ErrorCode.LoginFailException, 0);
        }
    }

    public async Task<ErrorCode> UpdateLastLoginTime(int uid)
    {
        try
        {
            int count = await _queryFactory.Query("user_info").Where("uid",uid).UpdateAsync(new
            {
                recent_login_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            });

            _logger.ZLogDebug($"[Login] uid: {uid}, count : {count}");

            return count != 1 ? ErrorCode.LoginUpdateRecentLoginFail : ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[AccountDb.Login] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFailException}, Uid: {uid}");
            return ErrorCode.CreateAccountFailException;
        }
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