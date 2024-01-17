using APIServer.Model.DAO.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class FriendListResponse : ErrorCodeDTO
{
    public IEnumerable<FriendUserInfo> FriendList { get; set; }
}
