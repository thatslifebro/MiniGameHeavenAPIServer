using System;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname);
    
    public Task<(ErrorCode, int)> VerifyUser(Int64 playerId);
    public Task<ErrorCode> UpdateLastLoginTime(int uid);
    public Task<ErrorCode> AddFriendByUid(int uid, int friendUid);
}