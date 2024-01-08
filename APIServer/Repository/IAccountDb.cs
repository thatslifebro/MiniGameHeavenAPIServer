using System;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(string id, string pw);
    
    public Task<Tuple<ErrorCode, Int64>> VerifyUser(string email, string pw);
}