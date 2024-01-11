using APIServer.Controllers.Auth;
using APIServer.Model.DAO;
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

namespace APIServer.Servicies
{
    public class AuthService : IAuthService
    {
        readonly ILogger<AuthService> _logger;
        readonly IAccountDb _accountDb;
        readonly IMemoryDb _memoryDb;
        string _hiveServerAddress;
        public AuthService(ILogger<AuthService> logger, IConfiguration configuration, IAccountDb accountDb, IMemoryDb memoryDb)
        {
            _memoryDb = memoryDb;
            _accountDb = accountDb;
            _logger = logger;
            _hiveServerAddress = configuration.GetSection("HiveServerAddress").Value + "/verifytoken";
        }
        public async Task<bool> VerifyTokenToHive(Int64 playerId, string token)
        {
            try
            {
                HttpClient client = new();
                var hiveResponse = await client.PostAsJsonAsync(_hiveServerAddress, new { PlayerId = playerId, HiveToken = token });

                if (!ValidateHiveResponse(hiveResponse))
                {
                    var statusCode = hiveResponse == null ? 0 : hiveResponse.StatusCode;
                    _logger.ZLogDebug($"[VerifyTokenToHive Service] ErrorCode:{ErrorCode.Hive_Fail_InvalidResponse}, PlayerID = {playerId}, Token = {token}, StatusCode = {statusCode}");
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

        public async Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname)
        {
            try
            {
                //playerId 중복 체크
                var existUser = await _accountDb.GetUserByPlayerId(playerId);
                if (existUser is not null)
                {
                    _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountAlreadyExistFail}, PlayerId : {playerId}");
                    return ErrorCode.CreateAccountDuplicateFail;
                }
                //nickname 중복 체크
                existUser = await _accountDb.GetUserByNickname(nickname);
                if (existUser is not null)
                {
                    _logger.ZLogError($"[CreateAccount] ErrorCode: {ErrorCode.CreateAccountDuplicateFail}, nickname : {nickname}");
                    return ErrorCode.CreateAccountDuplicateFail;
                }
                //account 생성
                int count = await _accountDb.InsertUser(playerId, nickname);

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
                //playerId로 userInfo 조회
                AdbUserInfo userInfo = await _accountDb.GetUserByPlayerId(playerId);
                //없는 유저라면 에러
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

        public async Task<ErrorCode> UpdateLastLoginTime(int uid)
        {
            try
            {
                int count = await _accountDb.UpdateRecentLogin(uid);

                return count != 1 ? ErrorCode.LoginUpdateRecentLoginFail : ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[AccountDb.Login] ErrorCode: {ErrorCode.LoginUpdateRecentLoginFailException}, Uid: {uid}");
                return ErrorCode.CreateAccountFailException;
            }
        }

        public bool ValidateHiveResponse(HttpResponseMessage? response)
        {
            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
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
}
