using APIServer.Model.DAO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class GameListRequest : RequestDTO
{
}


public class GameListResponse : ErrorCodeDTO
{
    public IEnumerable<GdbGameInfo> GameList { get; set; }
}
