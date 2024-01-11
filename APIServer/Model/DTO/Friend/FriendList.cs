using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class FriendListRequest : RequestDTO
{
}


public class FriendListResponse : ErrorCodeDTO
{
    public IEnumerable<FriendUserInfo> FriendList { get; set; }
}

public class FriendUserInfo
{ 
    public int Uid { get; set; }
    public string NickName { get; set; }
    // 데이터 추가 필요 - 대표 캐릭터 정보, 역대 최고 점수, 최근 접속 일자 등
}
