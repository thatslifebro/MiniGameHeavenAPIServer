using APIServer.Model.DAO.GameDB;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IAttendanceService
    {
        public Task<(ErrorCode, GdbAttendanceInfo)> GetAttendance(int uid);
    }
}
