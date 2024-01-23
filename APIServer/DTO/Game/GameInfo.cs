using APIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class GameInfoRequest
{
    [Required]
    public int GameKey { get; set; }
}


public class GameInfoResponse : ErrorCodeDTO
{
    public GdbGameInfo GameInfo { get; set; }
}
