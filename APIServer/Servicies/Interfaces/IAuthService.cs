using System.Threading.Tasks;
using System;

namespace APIServer.Servicies.Interfaces;

public interface IAuthService
{
    public Task<bool> VerifyTokenToHive(Int64 playerId, string hiveToken);
    public Task<(ErrorCode,int)> CreateAccountAsync(Int64 playerId, string nickname);
    public  Task<ErrorCode> DeleteAccountAsync(int uid);
    public Task<(ErrorCode, int)> VerifyUser(Int64 playerId);
    public Task<ErrorCode> UpdateLastLoginTime(int uid);
}
