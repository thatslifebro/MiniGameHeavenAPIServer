using APIServer.Model.DAO;
using System.Threading.Tasks;

namespace APIServer.Repository;

public interface IMemoryDb
{
    public Task<ErrorCode> RegistUserAsync(string token, int uid);
    public Task<ErrorCode> CheckUserAuthAsync(string id, string authToken);
    public Task<(bool, RdbAuthUserData)> GetUserAsync(int id);
    public Task<bool> SetUserStateAsync(RdbAuthUserData user, UserState userState);
    public Task<bool> SetUserReqLockAsync(string key);
    public Task<bool> DelUserReqLockAsync(string key);
    public  Task<ErrorCode> DelUserAuthAsync(int uid);
}
