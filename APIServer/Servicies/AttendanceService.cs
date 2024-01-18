using APIServer.MasterData;
using APIServer.Model.DAO.GameDB;
using APIServer.Repository;
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
    readonly IMasterDb _masterDb;
    public AttendanceService(ILogger<AttendanceService> logger, IGameDb gameDb, IMasterDb masterDb)
    {
        _logger = logger;
        _gameDb = gameDb;
        _masterDb = masterDb;
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

    public async Task<ErrorCode> CheckAttendance(int uid)
    {
        try
        {
            var rowCount = await _gameDb.CheckAttendanceById(uid);
            if (rowCount != 1)
            {
                return ErrorCode.AttendanceCheckFailAlreadyChecked;
            }

            var attendanceInfo = await _gameDb.GetAttendanceById(uid);
            var attendanceCnt = attendanceInfo.attendance_cnt;

            //출석 보상 수령 - 한 종류의 보상만 받기 때문에 reward는 1개.
            var a = _masterDb._attendanceRewardList;
            var reward = _masterDb._attendanceRewardList.Find(reward => reward.day_seq == attendanceCnt);
            await GetReward(uid, reward);

            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Attendance.CheckAttendance] ErrorCode: {ErrorCode.AttendanceCheckFailException}, Uid: {uid}");
            return ErrorCode.AttendanceCheckFailException;
        }

        async Task<ErrorCode> GetReward(int uid, AttendanceRewardData reward)
        {
            try
            {
                var rowCount = 0;
                switch (reward.reward_key)
                {
                    case 1: //보석
                        rowCount = await _gameDb.UpdateUserjewelry(uid, reward.reward_qty);
                        break;
                    case < 1000:
                        break;
                    case < 2000: //캐릭터
                        //GetChar Service
                        //캐릭터가 없다면 insert, 있다면 캐릭터 개수 늘려주고 레벨업.
                        break;
                    case < 3000: //스킨
                        //GetSkin Service
                        break;
                    case < 4000: //코스튬
                        //GetCostume Service
                        break;
                    case < 5000: //푸드
                        //GetFood Service
                        break;
                    case < 6000: // 가챠
                        //GetGacha Service
                        break;
                }
            } catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[GetReward] ErrorCode: {ErrorCode.GetRewardFailException}, Uid: {uid}");
                return ErrorCode.GetRewardFailException;
            }

            return ErrorCode.None;
        }
    }
}


