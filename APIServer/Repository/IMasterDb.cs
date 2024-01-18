using APIServer.MasterData;
using APIServer.Model.DAO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Repository;

public interface IMasterDb
{
    public VersionDAO _version { get; }
    public List<AttendanceRewardData> _attendanceRewardList { get; }
    public List<CharacterData> _characterList { get; }
    public List<SkinData> _skinList { get; }
    public List<CostumeData> _costumeList { get; }
    public List<CostumeSetData> _costumeSetList { get; }
    public List<FoodData> _foodList { get; }
    public List<SkillData> _skillList { get; }
    public List<GachaRewardData> _gachaRewardList { get; }

    public Task<bool> Load();
}
