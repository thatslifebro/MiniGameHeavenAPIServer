using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIServer.Model.DAO.GameDB;
using SqlKata.Execution;

namespace APIServer.Services;

public partial class GameDb : IGameDb
{
    #region Character
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

    public async Task<GdbUserCharInfo> GetCharInfo(int uid, int charKey)
    {
        return await _queryFactory.Query("user_char").Where("uid", uid)
                                                .Where("char_key", charKey)
                                                .FirstOrDefaultAsync<GdbUserCharInfo>();
    }

    public async Task<int> InsertUserChar(int uid, int charKey)
    {
        return await _queryFactory.Query("user_char").InsertAsync(new { uid, char_key = charKey, create_dt = DateTime.Now});
    }

    public async Task<int> LevelUpChar(int uid, int charKey)
    {
        return await _queryFactory.StatementAsync($"UPDATE user_char SET level = level + 1, char_cnt = 0 WHERE uid = {uid} AND char_key = {charKey}");
    }

    public async Task<int> IncrementCharCnt(int uid, int charKey)
    {
        return await _queryFactory.Query("user_char").Where("uid", uid)
                                                .Where("char_key", charKey)
                                                .IncrementAsync("char_cnt");
    }

    #endregion

    #region Skin

    public async Task<IEnumerable<GdbUserSkinInfo>> GetSkinList(int uid)
    {
        return await _queryFactory.Query("user_skin").Where("uid", uid)
                                                .OrderBy("skin_key")
                                                .GetAsync<GdbUserSkinInfo>();
    }

    public async Task<GdbUserSkinInfo> GetSkinInfo(int uid, int skinKey)
    {
        return await _queryFactory.Query("user_skin").Where("uid", uid)
                                                .Where("skin_key", skinKey)
                                                .FirstOrDefaultAsync<GdbUserSkinInfo>();
    }

    public async Task<int> InsertUserSkin(int uid, int skinKey)
    {
        return await _queryFactory.Query("user_skin").InsertAsync(new { uid, skin_key = skinKey, create_dt = DateTime.Now});
    }

    #endregion

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
}