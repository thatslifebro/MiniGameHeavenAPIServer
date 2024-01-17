using System.Threading.Tasks;
using APIServer.Model.DAO.GameDB;
using SqlKata.Execution;

namespace APIServer.Services;

public partial class GameDb : IGameDb
{
    public async Task<GdbAttendanceInfo> GetAttendanceById(int uid)
    {
        return await _queryFactory.Query("user_attendance").Where("uid", uid)
                                                .FirstOrDefaultAsync<GdbAttendanceInfo>();
    }
}