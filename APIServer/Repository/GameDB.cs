//TODO 사용하지 않는 코드 정리하기
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using APIServer.Controllers.Game;
using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using APIServer.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace APIServer.Services;


public class GameDb : IGameDb
{
    readonly ILogger<GameDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;

    const int InitCharacterKey = 1;
    IDbConnection _dbConn;
    SqlKata.Compilers.MySqlCompiler _compiler;
    QueryFactory _queryFactory;
    
    public GameDb(ILogger<GameDb> logger, IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new SqlKata.Execution.QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    public async Task<AdbUserInfo> GetUserByPlayerId(Int64 playerId)
    {
        return await _queryFactory.Query("user")
                                .Where("player_id", playerId)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<AdbUserInfo> GetUserByUid(int uid)
    {
        return await _queryFactory.Query("user")
                                .Where("uid", uid)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<AdbUserInfo> GetUserByNickname(string nickname)
    {
        return await _queryFactory.Query("user")
                                .Where("nickname", nickname)
                                .FirstOrDefaultAsync<AdbUserInfo>();
    }

    public async Task<int> InsertUser(Int64 playerId, string nickname)
    {
        return await _queryFactory.Query("user")
                                .InsertGetIdAsync<int>(new
                                {
                                    player_id = playerId,
                                    nickname = nickname,
                                    create_dt = DateTime.Now,
                                    recent_login_dt = DateTime.Now,
                                });
    }

    public async Task<int> DeleteAccount(int uid)
    {
        return await _queryFactory.Query("user")
                                .Where("uid", uid)
                                .DeleteAsync();
    }

    public IDbConnection ADbConnection()
    {
        return _queryFactory.Connection;
    }

    public async Task<int> UpdateRecentLogin(int uid)
    {
        return await _queryFactory.Query("user").Where("uid", uid).UpdateAsync(new
        {
            recent_login_dt = DateTime.Now,
        });
    }

    public async Task<AdbFriendReqInfo> GetFriendReqInfo(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .FirstOrDefaultAsync<AdbFriendReqInfo>();
    }

    public async Task<int> InsertFriendReq(int uid, int friendUid, bool accept = false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    accept_yn = accept,
                                    create_dt = DateTime.Now,
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
                                    create_dt = DateTime.Now,
                                }, transaction);
    }

    public async Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend").Where("uid", friendUid).Where("friend_uid", uid).UpdateAsync(new
        {
            accept_yn = accept,
        }, transaction);
    }

    public async Task<IEnumerable<AdbFriendUserInfo>> GetFriendUserInfoList(int uid, string orderby)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", true)
                                .Select("user.uid", "nickname", $"{orderby}", "recent_login_dt")// AdbFriendUserInfo에 따라 변경 필요
                                .OrderByDesc(orderby)
                                .OrderBy("nickname")
                                .GetAsync<AdbFriendUserInfo>();
    }

    public async Task<IEnumerable<AdbFriendReqListInfo>> GetFriendReceivedReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.uid")
                                .Where("friend.friend_uid", uid)
                                .Where("accept_yn", false)
                                .Select("user.uid", "user.nickname", "friend.create_dt")
                                .GetAsync<AdbFriendReqListInfo>();
    }

    public async Task<IEnumerable<AdbFriendReqListInfo>> GetFriendSentReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", false)
                                .Select("user.uid", "user.nickname", "friend.create_dt")
                                .GetAsync<AdbFriendReqListInfo>();
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
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .DeleteAsync();
    }

    public async Task<IEnumerable<GdbGameInfo>> GetGameList(int uid)
    {
        return await _queryFactory.Query("user_game")
            .Where("uid", uid)
            .OrderBy("game_key")
            .GetAsync<GdbGameInfo>();
    }

    public async Task<int> InsertInitGameList(int uid, IDbTransaction transaction)
    {
        var now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return await _queryFactory.Query("user_game").InsertAsync(new[] { "uid", "game_key", "create_dt", "char_key" }, new[]
        {
            new object[]{uid,1,now, InitCharacterKey},
            new object[]{uid,2,now, InitCharacterKey},
            new object[]{uid,3,now, InitCharacterKey},
        },transaction);
    }

    public async Task<int> InsertGame(int uid,int gameKey)
    {
        return await _queryFactory.Query("user_game").InsertAsync(
            new { uid = uid,
                char_key = InitCharacterKey,
                game_key = gameKey,
                create_dt = DateTime.Now});
    }

    public async Task<GdbGameInfo> GetGameInfo(int uid, int gameKey)
    {
        return await _queryFactory.Query("user_game").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .FirstOrDefaultAsync<GdbGameInfo>();
    }

    public async Task<int> UpdateBestscore(int uid, int gameKey, int score)
    {
        return await _queryFactory.Query("user_game").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .Where("bestscore", "<", score)
                                                .UpdateAsync(new 
                                                { 
                                                    bestscore = score,
                                                    bestscore_cur_season = score,
                                                    new_record_dt = DateTime.Now,
                                                    recent_play_dt = DateTime.Now
                                                });
    }

    public async Task<int> UpdateBestscoreCurSeason(int uid, int gameKey, int score)
    {
        return await _queryFactory.Query("user_game").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .Where("bestscore_cur_season", "<", score)
                                                .UpdateAsync(new
                                                {
                                                    bestscore_cur_season = score,
                                                    recent_play_dt = DateTime.Now
                                                });
    }

    public async Task<int> UpdateRecentPlayDt(int uid, int gameKey)
    {
        return await _queryFactory.Query("user_game").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .UpdateAsync(new
                                                {
                                                    recent_play_dt = DateTime.Now
                                                });
    }

    public async Task<int> InsertInitCharacter(int uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_char").InsertAsync(
            new
            {
                uid = uid,
                char_key = InitCharacterKey,
                create_dt = DateTime.Now
            }, transaction);
    }

    public IDbConnection GDbConnection()
    {
        return _queryFactory.Connection;
    }

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.GameDb);

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
    public string GameDb { get; set; }
    public string Redis { get; set; }
}