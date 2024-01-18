using APIServer.MasterData;
using System.Collections.Generic;

namespace APIServer.Model.DTO.Attendance
{
    public class AttendanceCheckResponse : ErrorCodeDTO
    {
        public RewardData Reward { get; set; }
    }
}
