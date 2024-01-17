using APIServer.Model.DAO.GameDB;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class AttendanceService : IAttendanceService
{
    readonly ILogger<AttendanceService> _logger;
    readonly IGameDb _gameDb;
    public AttendanceService(ILogger<AttendanceService> logger, IGameDb gameDb)
    {
        _logger = logger;
        _gameDb = gameDb;
    }
    public async Task<(ErrorCode, GdbAttendanceInfo)> GetAttendance(int uid)
    {
        try
        {
            return (ErrorCode.None, await _gameDb.GetAttendanceById(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                                   $"[User.GetUserInfo] ErrorCode: {ErrorCode.AttendanceInfoFailException}, Uid: {uid}");
            return (ErrorCode.AttendanceInfoFailException, null);
        }
    }
}


