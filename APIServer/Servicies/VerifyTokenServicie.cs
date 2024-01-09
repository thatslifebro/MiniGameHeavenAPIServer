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
    public class VerifyTokenServicie : IVerifyTokenService
    {
        readonly ILogger<VerifyTokenServicie> _logger;
        string _hiveServerAddress;
        public VerifyTokenServicie(ILogger<VerifyTokenServicie> logger, IConfiguration configuration)
        {
            _logger = logger;
            _hiveServerAddress = configuration.GetSection("HiveServerAddress").Value+ "/verifytoken";
        }

        public async Task<bool> VerifyTokenToHive(Int64 playerId, string token)
        {
            try
            {
                HttpClient client = new();
                var hiveResponse = await client.PostAsJsonAsync(_hiveServerAddress, new { PlayerId = playerId, Token = token });

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

        public bool ValidateHiveResponse(HttpResponseMessage? response)
        {
            if (response == null || response.StatusCode !=System.Net.HttpStatusCode.OK)
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
