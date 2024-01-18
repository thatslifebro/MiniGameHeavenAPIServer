using APIServer.Model.DAO.GameDB;

namespace APIServer.Model.DTO.Attendance;

public class AttendanceInfoResponse : ErrorCodeDTO
{
    public GdbAttendanceInfo AttendanceInfo { get; set; }
}