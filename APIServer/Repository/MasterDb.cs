using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APIServer.MasterData;
using APIServer.Model.DAO;
using APIServer.Services;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace APIServer.Repository;

public class MasterDb : IMasterDb
{
    readonly Dictionary<long, ItemData> _itemDatas = new();
    readonly IOptions<DbConfig> _dbConfig;
    readonly ILogger<MasterDb> _logger;
    IDbConnection _dbConn;
    readonly SqlKata.Compilers.MySqlCompiler _compiler;
    readonly QueryFactory _queryFactory;
    
    public VersionDAO? _version { get; set; }

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
            //_ = LoadItem(queryFactory);
            _version = await _queryFactory.Query($"version").FirstOrDefaultAsync<VersionDAO>();
        }
        catch(Exception e)
        {
            _logger.ZLogError(e,
                $"[MasterDb.Load] ErrorCode: {ErrorCode.MasterDB_Fail_LoadData}");
            return false;
        }
        if(!ValidateMasterData())
        {
            _logger.ZLogError($"[MasterDb.Load] ErrorCode: {ErrorCode.MasterDB_Fail_InvalidData}");
            return false;
        }

        return true;

    }

    bool ValidateMasterData()
    {
        if (_version == null)
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
