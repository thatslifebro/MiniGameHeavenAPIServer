using System.Collections.Generic;
using System.Threading.Tasks;
using APIServer.Model.DAO.GameDB;
using SqlKata.Execution;

namespace APIServer.Services;

public partial class GameDb : IGameDb
{
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
}