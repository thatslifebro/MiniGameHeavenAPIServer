using System;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname);
    
    public Task<Tuple<ErrorCode, Int64>> VerifyUser(string email, string pw);
}