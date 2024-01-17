using System;

namespace APIServer.Model.DAO.GameDB
{
    public class GdbAttendance
    {
        public int uid { get; set; }
        public int attendance_count { get; set; }
        public DateTime recent_attendance_dt { get; set; }
    }
}
