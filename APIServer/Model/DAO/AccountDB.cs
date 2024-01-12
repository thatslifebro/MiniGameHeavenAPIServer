using System.Xml;

namespace APIServer.Model.DAO;

//AccountDB의 객체는 객체 이름 앞에 Adb를 붙인다.

public class AdbUserInfo
{
    public int uid { get; set; }

    public string player_id { get; set; }
    public string create_dt { get; set; }
    public string recent_login_dt { get; set; }
    public string nickname { get; set; }
}

public class AdbFriendReqInfo
{
    public int uid { get; set; }

    public string friend_uid { get; set; }
    public bool accept_yn { get; set; }
    public string create_dt { get; set; }
}

public class AdbFriendUserInfo
{
    public int uid { get; set; }
    public string nickname { get; set; }
    public int bestscore { get; set; }
    public string recent_login_dt { get; set; }
    // 데이터 추가 필요 - 대표 캐릭터 정보, 최근 접속 일자 등
}

public class AdbFriendReqListInfo
{
    public int uid { get; set; }
    public string nickname { get; set; }
    public string create_dt { get; set; }
}