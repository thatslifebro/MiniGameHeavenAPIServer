using APIServer.Model.DAO.GameDB;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IUserService
    {
        public Task<(ErrorCode, GdbUserInfo)> GetUserInfo(int uid);
    }
}
