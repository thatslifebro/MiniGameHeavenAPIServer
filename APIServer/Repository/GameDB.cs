//TODO 사용하지 않는 코드 정리하기
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
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

    public async Task<IEnumerable<GdbGameListInfo>> GetGameList(int uid)
    {
        return await _queryFactory.Query("game").Join("master_db.game_info", "game.game_id", "game_info.game_id")
            .Select("game.game_id", "game_name", "bestscore","game.create_dt")
            .Where("uid", uid)
            .OrderBy("uid")
            .GetAsync<GdbGameListInfo>();
    }

    public async Task<int> InsertInitGameList(int uid)
    {
        var now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return await _queryFactory.Query("game").InsertAsync(new[] { "uid", "game_id", "create_dt" }, new[]
        {
            new object[]{uid,1,now},
            new object[]{uid,2,now},
            new object[]{uid,3,now},
        });
    }

    public async Task<int> InsertGame(int uid,int gameId)
    {
        return await _queryFactory.Query("game").InsertAsync(
            new { uid = uid,
                game_id = gameId,
                create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")});
    }

    public async Task<GdbGameInfo> GetGameInfo(int uid, int gameId)
    {
        return await _queryFactory.Query("game").Join("master_db.game_info", "game.game_id", "game_info.game_id")
                                            .Select("game.game_id", "game_name", "bestscore","game.create_dt", "new_record_dt", "recent_play_dt") // GdbGameInfo 추가예정
                                            .Where("uid", uid)
                                            .Where("game.game_id", gameId)
                                            .FirstOrDefaultAsync<GdbGameInfo>();
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