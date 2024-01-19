
using APIServer.Models;
using System.Collections.Generic;

namespace APIServer.DTO.Game;

public class GameSaveRequest
{
    public int GameKey { get; set; }
    public int Score { get; set; }
    public List<UsedFoodData> Foods { get; set; }
}

public class UsedFoodData
{
    public int FoodKey { get; set; }
    public int FoodQty { get; set; }
}


public class GameSaveResponse : ErrorCodeDTO
{
}
