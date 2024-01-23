﻿using APIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Friend;

public class FriendRequestListResponse : ErrorCodeDTO
{
    public IEnumerable<FriendReqListInfo> FriendRequestList { get; set; }
}