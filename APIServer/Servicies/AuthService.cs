using APIServer.Controllers.Auth;
using APIServer.Model.DAO.GameDB;
using APIServer.Repository;
using APIServer.Services;
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
    public async Task<bool> VerifyTokenToHive(Int64 playerId, string token)
    {
        try
        {
            HttpClient client = new();
            var hiveResponse = await client.PostAsJsonAsync(_hiveServerAddress, new { PlayerId = playerId, HiveToken = token });

            if (hiveResponse == null || !ValidateHiveResponse(hiveResponse))
            {
                _logger.ZLogDebug($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.Hive_Fail_InvalidResponse}, PlayerID = {playerId}, Token = {token}, StatusCode = {hiveResponse?.StatusCode}");
                return false;
            }

            var authResult = await hiveResponse.Content.ReadFromJsonAsync<ErrorCodeDTO>();
            return ValidateHiveAuthErrorCode(authResult);
        }
        catch
        {
            _logger.ZLogDebug($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.Hive_Fail_InvalidResponse}, PlayerID = {playerId}, Token = {token}");
            return false;
        }
    }

    public async Task<(ErrorCode, int)> CreateAccountAsync(Int64 playerId, string nickname)
    {
        try
        {
            if(string.IsNullOrEmpty(nickname))
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountNicknameFail}, nickname : {nickname}");
                return (ErrorCode.CreateAccountNicknameFail,0);
            }
            //nickname 중복 체크
            var existUser = await _gameDb.GetUserByNickname(nickname);
            if (existUser is not null)
            {
                _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountDuplicateFail}, nickname : {nickname}");
                return (ErrorCode.CreateAccountDuplicateFail,0);
            }
            //account 생성
            return (ErrorCode.None, await _gameDb.InsertUser(playerId, nickname));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountFailException}, PlayerId: {playerId}");
            return (ErrorCode.CreateAccountFailException,0);
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
            int count = await _gameDb.UpdateRecentLogin(uid);

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
            return ErrorCode.CreateAccountFailException;
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
