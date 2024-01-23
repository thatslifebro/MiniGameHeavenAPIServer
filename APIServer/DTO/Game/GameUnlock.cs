using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game;

public class GameUnlockRequest
{
    [Required]
    public int GameKey { get; set; }
}


public class GameUnlockResponse : ErrorCodeDTO
{
}
