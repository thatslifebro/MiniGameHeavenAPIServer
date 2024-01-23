﻿using APIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Friend;

public class FriendListResponse : ErrorCodeDTO
{
    public IEnumerable<GdbFriendInfo> FriendList { get; set; }
}
