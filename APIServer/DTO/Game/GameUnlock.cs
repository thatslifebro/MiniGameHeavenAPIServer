namespace APIServer.DTO.Game;

public class GameUnlockRequest
{
    public int GameKey { get; set; }
}


public class GameUnlockResponse : ErrorCodeDTO
{
}
