using System;

namespace APIServer.Models.GameDB
{

    public class GdbFriendInfo
    {
        public int uid { get; set; }

        public string friend_uid { get; set; }
        public bool friend_yn { get; set; }
        public DateTime create_dt { get; set; }
    }

    public class FriendUserInfo
    {
        public int uid { get; set; }
        public string nickname { get; set; }
        public int bestscore { get; set; }
        public DateTime recent_login_dt { get; set; }
    }

    public class FriendReqListInfo
    {
        public int uid { get; set; }
        public string nickname { get; set; }
        public DateTime create_dt { get; set; }
    }
}
