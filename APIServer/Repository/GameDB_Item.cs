﻿using System;
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

    public async Task<int> InsertUserChar(int uid, int charKey, int cnt)
    {
        return await _queryFactory.Query("user_char").InsertAsync(new { uid, char_key = charKey,char_cnt = cnt, create_dt = DateTime.Now});
    }

    public async Task<int> LevelUpChar(int uid, int charKey, int level, int cnt)
    {
        return await _queryFactory.Query("user_char").Where("uid", uid)
                                                .Where("char_key", charKey)
                                                .UpdateAsync(new { char_level = level, char_cnt = cnt });
    }

    public async Task<int> IncrementCharCnt(int uid, int charKey, int qty)
    {
        return await _queryFactory.Query("user_char").Where("uid", uid)
                                                .Where("char_key", charKey)
                                                .IncrementAsync("char_cnt", qty);
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

    #region Costume

    public async Task<IEnumerable<GdbUserCostumeInfo>> GetCostumeList(int uid)
    {
        return await _queryFactory.Query("user_costume").Where("uid", uid)
                                                .OrderBy("costume_key")
                                                .GetAsync<GdbUserCostumeInfo>();
    }

    public async Task<GdbUserCostumeInfo> GetCostumeInfo(int uid, int costumeKey)
    {
        return await _queryFactory.Query("user_costume").Where("uid", uid)
                                                .Where("costume_key", costumeKey)
                                                .FirstOrDefaultAsync<GdbUserCostumeInfo>();
    }

    public async Task<int> InsertUserCostume(int uid, int costumeKey, int cnt)
    {
        return await _queryFactory.Query("user_costume").InsertAsync(new { uid, costume_key = costumeKey, costume_cnt = cnt, create_dt = DateTime.Now });
    }

    public async Task<int> LevelUpCostume(int uid, int costumeKey, int level, int cnt)
    {
        return await _queryFactory.Query("user_costume").Where("uid", uid)
                                                .Where("costume_key", costumeKey)
                                                .UpdateAsync(new { costume_level = level, costume_cnt = cnt });
    }

    public async Task<int> IncrementCostumeCnt(int uid, int costumeKey, int qty)
    {
        return await _queryFactory.Query("user_costume").Where("uid", uid)
                                                .Where("costume_key", costumeKey)
                                                .IncrementAsync("costume_cnt", qty);
    }

    #endregion

    public async Task<IEnumerable<GdbUserFoodInfo>> GetFoodList(int uid)
    {
        return await _queryFactory.Query("user_food").Where("uid", uid)
                                                .OrderBy("food_key")
                                                .GetAsync<GdbUserFoodInfo>();
    }
}