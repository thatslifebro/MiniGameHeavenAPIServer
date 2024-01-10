﻿using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APIServer.Model.DAO;
using APIServer.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using SqlKata.Execution;
using ZLogger;

namespace APIServer.Repository;

public class AccountDb : IAccountDb
{
    private readonly IOptions<DbConfig> _dbConfig;
    private readonly ILogger<AccountDb> _logger;
    private IDbConnection _dbConn;
    private readonly SqlKata.Compilers.MySqlCompiler _compiler;
    private readonly QueryFactory _queryFactory;

    public AccountDb(ILogger<AccountDb> logger, IOptions<DbConfig> dbConfig)
    {
        _dbConfig = dbConfig;
        _logger = logger;

        Open();

        _compiler = new SqlKata.Compilers.MySqlCompiler();
        _queryFactory = new QueryFactory(_dbConn, _compiler);
    }

    public void Dispose()
    {
        Close();
    }

    public async Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname)
    {
        try
        {
            var exist = await _queryFactory.Query("user_info").Where("player_id", playerId).GetAsync<int>();
            if (exist.Count()!=0)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountDuplicateFail}, PlayerId : {playerId}");
                return ErrorCode.CreateAccountDuplicateFail;
            }
            
            int count = await _queryFactory.Query("user_info").InsertAsync(new
            {
                uid = 0,
                player_id = playerId,
                nickname = nickname,
                create_dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            });

            _logger.ZLogDebug($"[CreateAccount] PlayerId: {playerId}");

            return count != 1 ? ErrorCode.CreateAccountFailInsert : ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[AccountDb.CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, PlayerId: {playerId}");
            return ErrorCode.CreateAccountFailException;
        }
    }

    public async Task<(ErrorCode, int)> VerifyUser(Int64 playerId)
    {
        try
        {
            AdbUserInfo userInfo = await _queryFactory.Query("user_info")
                                    .Where("player_id", playerId)
                                    .FirstOrDefaultAsync<AdbUserInfo>();

            if (userInfo is null)
            {
                return (ErrorCode.LoginFailUserNotExist, 0);
            }

            return (ErrorCode.None, userInfo.uid);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[AccountDb.VerifyAccount] ErrorCode: {ErrorCode.LoginFailException}, PlayerId: {playerId}");
            return (ErrorCode.LoginFailException, 0);
        }
    }

    private void Open()
    {
        _dbConn = new MySqlConnection(_dbConfig.Value.AccountDb);

        _dbConn.Open();
    }

    private void Close()
    {
        _dbConn.Close();
    }
}

public class DbConfig
{
    public string MasterDb { get; set; }
    public string AccountDb { get; set; }
    public string GameDb { get; set; }
    public string Memcached { get; set; }
}