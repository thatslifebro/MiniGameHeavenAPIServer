using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APIServer.MasterData;
using APIServer.Model.DAO;
using APIServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace APIServer.Repository;

public class MasterDb : IMasterDb
{
    
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<MasterDb> _logger;
    IDbConnection _dbConn;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;
    
    public VersionDAO? _version { get; set; }

    public List<AttendanceRewardData> _attendanceRewardList = new();
    public List<CharacterData> _characterList = new();
    public List<SkinData> _skinList = new();
    public List<CostumeData> _costumeList = new();
    public List<CostumeSetData> _costumeSetList = new();
    public List<FoodData> _foodList = new();
    public List<SkillData> _skillList = new();
    public List<GachaRewardData> _gachaRewardList = new();

    public MasterDb(ILogger<MasterDb> logger, IOptions<DbConfig> dbConfig)
    {
        _logger = logger;
        _dbConfig = dbConfig;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    public async Task<bool> Load()
    {
        try
        {
            _version = await _queryFactory.Query($"version").FirstOrDefaultAsync<VersionDAO>();
            _attendanceRewardList = (await _queryFactory.Query($"master_attendance_reward").GetAsync<AttendanceRewardData>()).ToList();
            _characterList = (await _queryFactory.Query($"master_char").GetAsync<CharacterData>()).ToList();
            _skinList = (await _queryFactory.Query($"master_skin").GetAsync<SkinData>()).ToList();
            _costumeList = (await _queryFactory.Query($"master_costume").GetAsync<CostumeData>()).ToList();
            _costumeSetList = (await _queryFactory.Query($"master_costume_set").GetAsync<CostumeSetData>()).ToList();
            _foodList = (await _queryFactory.Query($"master_food").GetAsync<FoodData>()).ToList();
            _skillList = (await _queryFactory.Query($"master_skill").GetAsync<SkillData>()).ToList();

            var gachaRewards = await _queryFactory.Query($"master_gacha_reward").GetAsync<GachaRewardInfo>();
            GachaRewardData gachaRewardData = new();
            foreach (var gachaRewardInfo in gachaRewards)
            {
                gachaRewardData.gachaRewardInfo = gachaRewardInfo;
                gachaRewardData.gachaRewardList = (await _queryFactory.Query("master_gacha_reward_list")
                                                   .Where("gacha_reward_key", gachaRewardInfo.gacha_reward_key)
                                                   .GetAsync<GachaReward>())
                                                   .ToList();
                _gachaRewardList.Add(gachaRewardData);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            _logger.ZLogError(e,
                $"[MasterDb.Load] ErrorCode: {ErrorCode.MasterDB_Fail_LoadData}");
            return false;
        }

        if (!ValidateMasterData())
        {
            _logger.ZLogError($"[MasterDb.Load] ErrorCode: {ErrorCode.MasterDB_Fail_InvalidData}");
            return false;
        }

        return true;
    }

    bool ValidateMasterData()
    {
        if (_version == null || 
            _attendanceRewardList.Count == 0 ||
            _characterList.Count == 0 ||
            _skinList.Count == 0 ||
            _costumeList.Count == 0 || 
            _costumeSetList.Count == 0 ||
            _foodList.Count == 0 ||
            _skillList.Count == 0 ||
            _gachaRewardList.Count == 0)
        {
            return false;
        }

        return true;
    }

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.MasterDb);

        _dbConn.Open();
    }

    private void Close()
    {
        _dbConn.Close();
    }
}
