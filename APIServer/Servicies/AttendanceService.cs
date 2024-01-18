using APIServer.MasterData;
using APIServer.Model.DAO.GameDB;
using APIServer.Repository;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies;

public class AttendanceService : IAttendanceService
{
    readonly ILogger<AttendanceService> _logger;
    readonly IGameDb _gameDb;
    readonly IMasterDb _masterDb;
    readonly IItemService _itemService;
    public AttendanceService(ILogger<AttendanceService> logger, IGameDb gameDb, IMasterDb masterDb, IItemService itemService)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
        _itemService = itemService;
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
                $"[Attendance.GetAttendance] ErrorCode: {ErrorCode.AttendanceInfoFailException}, Uid: {uid}");
            return (ErrorCode.AttendanceInfoFailException, null);
        }
    }

    public async Task<(ErrorCode, RewardData)> CheckAttendance(int uid)
    {
        try
        {
            var rowCount = await _gameDb.CheckAttendanceById(uid);
            if (rowCount != 1)
            {
                return (ErrorCode.AttendanceCheckFailAlreadyChecked, null);
            }

            var attendanceInfo = await _gameDb.GetAttendanceById(uid);
            var attendanceCnt = attendanceInfo.attendance_cnt;

            //출석 보상 수령 - 한 종류의 보상만 받기 때문에 reward는 1개.
            var a = _masterDb._attendanceRewardList;
            var reward = _masterDb._attendanceRewardList.Find(reward => reward.day_seq == attendanceCnt);
            await _itemService.GetReward(uid, reward);

            return (ErrorCode.None, reward);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Attendance.CheckAttendance] ErrorCode: {ErrorCode.AttendanceCheckFailException}, Uid: {uid}");
            return (ErrorCode.AttendanceCheckFailException, null);
        }

        
    }

    
}


