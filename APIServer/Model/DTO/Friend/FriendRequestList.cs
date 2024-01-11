using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class FriendRequestListRequest : RequestDTO
{
}


public class FriendRequestListResponse : ErrorCodeDTO
{
    public IEnumerable<FriendReqInfo> FriendRequestList { get; set; }
}

public class FriendReqInfo
{
    //uid와 nickname만 전달
    public int Uid { get; set; }
    public string NickName { get; set; }
}
