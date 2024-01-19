using APIServer.MasterData;
using APIServer.Model.DAO.GameDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IAttendanceService
    {
        public Task<(ErrorCode, GdbAttendanceInfo)> GetAttendance(int uid);
        public Task<(ErrorCode, List<ReceivedReward>)> CheckAttendance(int uid);
    }
}
