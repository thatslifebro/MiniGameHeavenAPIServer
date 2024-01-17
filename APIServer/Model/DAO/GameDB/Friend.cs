using System;

namespace APIServer.Model.DAO.GameDB
{

    public class GdbFriendInfo
    {
        public int uid { get; set; }

        public string friend_uid { get; set; }
        public bool accept_yn { get; set; }
        public DateTime create_dt { get; set; }
    }

    public class FriendUserInfo
    {
        public int uid { get; set; }
        public string nickname { get; set; }
        public int bestscore { get; set; }
        public DateTime recent_login_dt { get; set; }
        // 데이터 추가 필요 - 대표 캐릭터 정보, 최근 접속 일자 등
    }

    public class FriendReqListInfo
    {
        public int uid { get; set; }
        public string nickname { get; set; }
        public DateTime create_dt { get; set; }
    }
}
