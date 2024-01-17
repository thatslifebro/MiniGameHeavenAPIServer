//TODO 사용하지 않는 코드 정리하기
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APIServer.Model.DAO.GameDB;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;

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

    #region User
    public async Task<GdbUserInfo> GetUserByPlayerId(Int64 playerId)
    {
        return await _queryFactory.Query("user")
                                .Where("player_id", playerId)
                                .FirstOrDefaultAsync<GdbUserInfo>();
    }

    public async Task<GdbUserInfo> GetUserByUid(int uid)
    {
        return await _queryFactory.Query("user")
                                .Where("uid", uid)
                                .FirstOrDefaultAsync<GdbUserInfo>();
    }

    public async Task<GdbUserInfo> GetUserByNickname(string nickname)
    {
        return await _queryFactory.Query("user")
                                .Where("nickname", nickname)
                                .FirstOrDefaultAsync<GdbUserInfo>();
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

    public async Task<int> UpdateRecentLogin(int uid)
    {
        return await _queryFactory.Query("user").Where("uid", uid).UpdateAsync(new
        {
            recent_login_dt = DateTime.Now,
        });
    }
    #endregion

    #region Friend
    public async Task<GdbFriendInfo> GetFriendReqInfo(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .FirstOrDefaultAsync<GdbFriendInfo>();
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

    public async Task<IEnumerable<FriendUserInfo>> GetFriendUserInfoList(int uid, string orderby)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", true)
                                .Select("user.uid", "nickname", $"{orderby}", "recent_login_dt")// AdbFriendUserInfo에 따라 변경 필요
                                .OrderByDesc(orderby)
                                .OrderBy("nickname")
                                .GetAsync<FriendUserInfo>();
    }

    public async Task<IEnumerable<FriendReqListInfo>> GetFriendReceivedReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.uid")
                                .Where("friend.friend_uid", uid)
                                .Where("accept_yn", false)
                                .Select("user.uid", "user.nickname", "friend.create_dt")
                                .GetAsync<FriendReqListInfo>();
    }

    public async Task<IEnumerable<FriendReqListInfo>> GetFriendSentReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", false)
                                .Select("user.uid", "user.nickname", "friend.create_dt")
                                .GetAsync<FriendReqListInfo>();
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
    #endregion

    #region Game
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
    #endregion

    #region Init
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

    public async Task<int> InsertInitMoneyInfo(int uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_money").InsertAsync(
             new
             {
                uid = uid
             }, transaction);
    }

    public async Task<int> InsertInitAttendance(int uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_attendance").InsertAsync(
             new
             {
                uid = uid
             }, transaction);
    }
    #endregion

    #region ITEM
    public async Task<IEnumerable<GdbUserCharInfo>> GetCharList(int uid)
    {
        return await _queryFactory.Query("user_char").Where("uid", uid)
                                                .OrderBy("char_key")
                                                .GetAsync<GdbUserCharInfo>();
    }

    public async Task<IEnumerable<GdbUserCharRandomSkillInfo>> GetCharRandomSkillInfo(int uid, int charKey)
    {
        return await _queryFactory.Query("user_char_random_skill").Where("uid", uid)
                                                .Where("char_key", charKey)
                                                .GetAsync<GdbUserCharRandomSkillInfo>();
    }

    public async Task<IEnumerable<GdbUserSkinInfo>> GetSkinList(int uid)
    {
        return await _queryFactory.Query("user_skin").Where("uid", uid)
                                                .OrderBy("skin_key")
                                                .GetAsync<GdbUserSkinInfo>();
    }

    public async Task<IEnumerable<GdbUserCostumeInfo>> GetCostumeList(int uid)
    {
        return await _queryFactory.Query("user_costume").Where("uid", uid)
                                                .OrderBy("costume_key")
                                                .GetAsync<GdbUserCostumeInfo>();
    }

    public async Task<IEnumerable<GdbUserFoodInfo>> GetFoodList(int uid)
    {
        return await _queryFactory.Query("user_food").Where("uid", uid)
                                                .OrderBy("food_key")
                                                .GetAsync<GdbUserFoodInfo>();
    }
    #endregion

    public async Task<IEnumerable<GdbMailboxInfo>> GetMailList(int uid)
    {
        return await _queryFactory.Query("mailbox").Where("uid", uid)
                                                .OrderBy("mail_seq")
                                                .GetAsync<GdbMailboxInfo>();
    }

    public async Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int uid, int mailKey)
    {
        return await _queryFactory.Query("mailbox_reward").Where("uid", uid)
                                                .Where("mail_seq", mailKey)
                                                .GetAsync<GdbMailboxRewardInfo>();
    }

    public async Task<GdbAttendanceInfo> GetAttendanceById(int uid)
    {
        return await _queryFactory.Query("user_attendance").Where("uid", uid)
                                                .FirstOrDefaultAsync<GdbAttendanceInfo>();
    }
    
    public async Task<GdbUserMoneyInfo> GetUserMoneyById(int uid)
    {
        return await _queryFactory.Query("user_money").Where("uid", uid)
                                                .FirstOrDefaultAsync<GdbUserMoneyInfo>();
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