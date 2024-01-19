using APIServer.Models.GameDB;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class GameListResponse : ErrorCodeDTO
{
    public IEnumerable<GdbGameInfo> GameList { get; set; }
}
