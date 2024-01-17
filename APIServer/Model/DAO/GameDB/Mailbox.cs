using System;

namespace APIServer.Model.DAO.GameDB
{
    public class GdbMailboxInfo
    {
        public int mail_seq { get; set; }
        public int uid { get; set; }
        public string mail_title { get; set; }
        public DateTime create_dt { get; set; }
        public DateTime expire_dt { get; set; }
        public DateTime receive_dt { get; set; }
    }

    public class GdbMailboxRewardInfo
    {
        public int mail_seq { get; set; }
        public int reward_key { get; set; }
        public int reward_qty { get; set; }
        public int reward_type { get; set; }
    }
}
