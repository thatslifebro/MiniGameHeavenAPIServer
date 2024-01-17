using APIServer.Model.DTO.DataLoad;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IDataLoadService
    {
        public Task<(ErrorCode, UserDataLoadInfo)> LoadUserData(int uid);
    }
}
