using APIServer.MasterData;
using System.Collections.Generic;

namespace APIServer.Model.DTO.Attendance
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }
}
