using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using SqlKata.Execution;

namespace APIServer.Services;

//TODO 최흥배: 테이블의 모든 필드는 NULL을 가지지 않습니다. NULL을 의미하는 값을 대신 넣어주세요

//TODO 최흥배: user_game 에서 char_key는 어떤 의미일까요?
//김성연 : 각 게임마다 플레이할 캐릭터를 정할 수 있어서 넣었습니다.

//TODO 최흥배: user 와 user_game 테이블이 나누어질 이유는 없을 것 같습니다
//김성연 : user_game 테이블에 유저가 보유한 게임마다 row가 생성됩니다. 보유한 게임 정보, 게임마다 기록을 저장하기 위해서 나누었습니다.

public partial class GameDb : IGameDb
{
    public async Task<IEnumerable<GdbGameInfo>> GetGameList(int uid)
    {
        return await _queryFactory.Query("user_game")
            .Where("uid", uid)
            .OrderBy("game_key")
            .GetAsync<GdbGameInfo>();
    }

    public async Task<int> InsertInitGameList(int uid, int initCharKey, IDbTransaction transaction)
    {
        var now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return await _queryFactory.Query("user_game").InsertAsync(new[] { "uid", "game_key", "create_dt", "char_key" }, new[]
        {
            new object[]{uid,1,now, initCharKey},
            new object[]{uid,2,now, initCharKey},
            new object[]{uid,3,now, initCharKey},
        }, transaction);
    }

    public async Task<int> InsertGame(int uid, int initCharKey, int gameKey)
    {
        return await _queryFactory.Query("user_game").InsertAsync(
            new
            {
                uid = uid,
                char_key = initCharKey,
                game_key = gameKey,
                create_dt = DateTime.Now
            });
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

    public async Task<int> UpdateUserBestScoreEver(int uid)
    {
        return await _queryFactory.Query("user").Where("uid",uid).UpdateAsync(new
        {
            bestscore_ever = _queryFactory.Query("user_game").Where("uid", uid)
                                                             .Sum<int>("bestscore")
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

    public async Task<int> InsertInitCharacter(int uid, int initCharKey, IDbTransaction transaction)
    {
        return await _queryFactory.Query("user_char").InsertAsync(
            new
            {
                uid = uid,
                char_key = initCharKey,
                create_dt = DateTime.Now,
                costume_json = "{\"face\" : 0, \"hand\" : 0, \"head\" : 0}"
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
                 uid = uid,
                 recent_attendance_dt = DateTime.Now.AddDays(-1)
             }, transaction);
    }

    public async Task<int> UpdateGamePlayChar(int uid, int gameKey, int charKey)
    {
        return await _queryFactory.Query("user_game").Where("uid", uid)
                                                .Where("game_key", gameKey)
                                                .UpdateAsync(new
                                                {
                                                    char_key = charKey
                                                });
    }

    public async Task<int> FoodDecrement(int uid, int foodKey, int foodQty)
    {
        return await _queryFactory.Query("user_food").Where("uid", uid)
                                                .Where("food_key", foodKey)
                                                .Where("food_qty", ">=", foodQty)
                                                .DecrementAsync("food_qty", foodQty);
    }
}