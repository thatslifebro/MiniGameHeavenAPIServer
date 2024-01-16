using APIServer.Model.DAO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class GameUnlockRequest
{
    public int GameKey { get; set; }
}


public class GameUnlockResponse : ErrorCodeDTO
{
}
