using System.Collections;
using System.Collections.Generic;

namespace APIServer.Model.DTO.Friend;

public class FriendListRequest : RequestDTO
{
    public int FriendUid { get; set; }
}


public class FriendListResponse : ErrorCodeDTO
{
    List<FriendInfo> FriendList { get; set; }
}

public class FriendInfo
{ 
    public int Uid { get; set; }
    public string NickName { get; set; }
    // 데이터 추가 필요 - 대표 캐릭터 정보, 역대 최고 점수, 최근 접속 일자 등
}
