using APIServer.Model.DAO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Friend;

public class GameSaveRequest
{
    public int GameKey { get; set; }
    public int Score { get; set; }
    //아이템 사용 여부, 스타포인트, 금메달 
}


public class GameSaveResponse : ErrorCodeDTO
{
}
