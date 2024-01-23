using APIServer.Controllers.Auth;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class AuthService : IAuthService
{
    readonly ILogger<AuthService> _logger;
    readonly IGameDb _gameDb;
    readonly IMemoryDb _memoryDb;
    string _hiveServerAddress;
    public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IGameDb gameDb, IMemoryDb memoryDb)
    {
        _memoryDb = memoryDb;
        _gameDb = gameDb;
        _logger = logger;
        _hiveServerAddress = configuration.GetSection("HiveServerAddress").Value + "/verifytoken";
    }
    public async Task<ErrorCode> VerifyTokenToHive(Int64 playerId, string token)
    {
        try
        {
            HttpClient client = new();
            var hiveResponse = await client.PostAsJsonAsync(_hiveServerAddress, new { PlayerId = playerId, HiveToken = token });

            if (hiveResponse == null || !ValidateHiveResponse(hiveResponse))
            {
                _logger.ZLogDebug($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.Hive_Fail_InvalidResponse}, PlayerID = {playerId}, Token = {token}, StatusCode = {hiveResponse?.StatusCode}");
                return ErrorCode.Hive_Fail_InvalidResponse;
            }

            var authResult = await hiveResponse.Content.ReadFromJsonAsync<ErrorCodeDTO>();
            if (!ValidateHiveAuthErrorCode(authResult))
            {
                return ErrorCode.Hive_Fail_InvalidResponse;
            }

            return ErrorCode.None;
        }
        catch
        {
            _logger.ZLogDebug($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.Hive_Fail_InvalidResponse}, PlayerID = {playerId}, Token = {token}");
            return ErrorCode.Hive_Fail_InvalidResponse;
        }
    }

    public async Task<ErrorCode> DeleteAccountAsync(int uid)
    {
        try
        {
            // 게임 데이터도 다 지워야함.
            var rowCount = await _gameDb.DeleteAccount(uid);
            if(rowCount != 1)
            {
                _logger.ZLogDebug($"[DeleteAccountAsync] ErrorCode: {ErrorCode.DeleteAccountFail}, uid : {uid}");
                return ErrorCode.DeleteAccountFail;
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[DeleteAccountAsync] ErrorCode: {ErrorCode.DeleteAccountFailException}, uid: {uid}");
            return ErrorCode.DeleteAccountFailException;
        }
    }

    public async Task<(ErrorCode, int)> VerifyUser(Int64 playerId)
    {
        try
        {
            //playerId로 userInfo 조회
            GdbUserInfo userInfo = await _gameDb.GetUserByPlayerId(playerId);
            //없는 유저라면 생성.
            if (userInfo is null)
            {
                return (ErrorCode.LoginFailUserNotExist, 0);
            }

            return (ErrorCode.None, userInfo.uid);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[VerifyUser] ErrorCode: {ErrorCode.LoginFailException}, PlayerId: {playerId}");
            return (ErrorCode.LoginFailException, 0);
        }
    }

    public async Task<ErrorCode> UpdateLastLoginTime(int uid)
    {
        try
        {
            var count = await _gameDb.UpdateRecentLogin(uid);

            if (count != 1)
            {
                _logger.ZLogError($"[UpdateLastLoginTime] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFail}, count : {count}");
                return ErrorCode.LoginUpdateRecentLoginFail;
            }

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[UpdateLastLoginTime] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFailException}, Uid: {uid}");
            return ErrorCode.CreateUserFailException;
        }
    }

    public bool ValidateHiveResponse(HttpResponseMessage? response)
    {
        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return false;
        }
        return true;
    }

    bool ValidateHiveAuthErrorCode(ErrorCodeDTO? authResult)
    {
        if (authResult == null || authResult.Result != ErrorCode.None)
        {
            return false;
        }

        return true;
    }
}
