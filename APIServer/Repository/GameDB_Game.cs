using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using SqlKata.Execution;

namespace APIServer.Services;

public partial class GameDb : IGameDb
{
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
        }, transaction);
    }

    public async Task<int> InsertGame(int uid, int gameKey)
    {
        return await _queryFactory.Query("user_game").InsertAsync(
            new
            {
                uid = uid,
                char_key = InitCharacterKey,
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