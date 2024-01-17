using System;

namespace APIServer.Model.DAO.GameDB
{
    public class GdbUserInfo
    {
        public int uid { get; set; }
        public string player_id { get; set; }
        public string nickname { get; set; }
        public DateTime create_dt { get; set; }
        public DateTime recent_login_dt { get; set; }
        public int bestscore_ever { get; set; }
        public int bestscore_cur_season { get; set; }
        public int bestscore_prev_season { get; set; }
        public int star_point { get; set; }
    }

    public class GdbUserMoneyInfo
    {
        public int uid { get; set; }
        public int jewelry { get; set; }
        public int gold_medal { get; set; }
        public int sunchip { get; set; }
        public int cash { get; set; }
    }
}

