using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Threading.Tasks;
using APIServer.Models;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;

namespace APIServer.Services;
public partial class GameDb : IGameDb
{
    readonly ILogger<GameDb> _logger;
    readonly IOptions<DbConfig> _dbConfig;

    //TODO 최흥배: 이런 값을 코드에 넣으면 안됩니다. 마스터 데이터에서 읽어와서 사용해야 합니다. 프로그램은 데이터드리븐으로 동작하는 것이 좋습니다 
    const int InitCharacterKey = 1001;  
    
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

    public async Task<GdbUserMoneyInfo> GetUserMoneyById(int uid)
    {
        return await _queryFactory.Query("user_money").Where("uid", uid)
                                                .FirstOrDefaultAsync<GdbUserMoneyInfo>();
    }

    public async Task<int> UpdateUserjewelry(int uid, int rewardQty)
    {
        return await _queryFactory.Query("user_money").Where("uid", uid)
                                                .IncrementAsync("jewelry", rewardQty);
    }

    public async Task<IEnumerable<RdbUserScoreData>> SelectAllUserScore()
    {
        return await _queryFactory.Query("user").Select("uid", "bestscore_ever").GetAsync<RdbUserScoreData>();
    }

    public IDbConnection GDbConnection()
    {
        return _queryFactory.Connection;
    }

    void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.GameDb);

        _dbConn.Open();
    }

    void Close()
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