using APIServer.Model.DAO.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class GameInfoRequest
{
    public int GameKey { get; set; }
}


public class GameInfoResponse : ErrorCodeDTO
{
    public GdbGameInfo GameInfo { get; set; }
}
