﻿using APIServer.Model.DAO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class GameInfoRequest : RequestDTO
{
    public int GameId { get; set; }
}


public class GameInfoResponse : ErrorCodeDTO
{
    public GdbGameInfo GameInfo { get; set; }
}
