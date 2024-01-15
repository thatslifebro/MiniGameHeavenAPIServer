//TODO 사용하지 않는 코드 정리하기
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
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

    const int InitCharacterId = 1;
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

    public async Task<int> InsertInitGameList(int uid, IDbTransaction transaction)
    {
        var now = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        return await _queryFactory.Query("game").InsertAsync(new[] { "uid", "game_id", "create_dt" }, new[]
        {
            new object[]{uid,1,now},
            new object[]{uid,2,now},
            new object[]{uid,3,now},
        },transaction);
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
                                            .Select("game.game_id", "game_name", "bestscore","game.create_dt", "new_record_dt", "recent_play_dt", "bestscore_cur_season", "bestscore_prev_season") // GdbGameInfo 추가예정
                                            .Where("uid", uid)
                                            .Where("game.game_id", gameId)
                                            .FirstOrDefaultAsync<GdbGameInfo>();
    }

    public async Task<int> UpdateBestscore(int uid, int gameId, int score)
    {
        return await _queryFactory.Query("game").Where("uid", uid)
                                                .Where("game_id", gameId)
                                                .Where("bestscore", "<", score)
                                                .UpdateAsync(new 
                                                { 
                                                    bestscore = score,
                                                    bestscore_cur_season = score,
                                                    new_record_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                                    recent_play_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                                });
    }

    public async Task<int> UpdateBestscoreCurSeason(int uid, int gameId, int score)
    {
        return await _queryFactory.Query("game").Where("uid", uid)
                                                .Where("game_id", gameId)
                                                .Where("bestscore_cur_season", "<", score)
                                                .UpdateAsync(new
                                                {
                                                    bestscore_cur_season = score,
                                                    recent_play_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                                });
    }

    public async Task<int> UpdateRecentPlayDt(int uid, int gameId)
    {
        return await _queryFactory.Query("game").Where("uid", uid)
                                                .Where("game_id", gameId)
                                                .UpdateAsync(new
                                                {
                                                    recent_play_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
                                                });
    }

    public async Task<int> InsertInitCharacter(int uid, IDbTransaction transaction)
    {
        return await _queryFactory.Query("char_info").InsertAsync(
            new
            {
                uid = uid,
                char_id = InitCharacterId,
                create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            }, transaction);
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