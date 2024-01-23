using System.ComponentModel.DataAnnotations;

namespace APIServer.DTO.Game
{
    public class GameSetPlayCharRequest
    {
        [Required]
        public int GameKey { get; set; }
        [Required]
        public int CharKey { get; set; }
    }

    public class GameSetPlayCharResponse : ErrorCodeDTO
    {
    }
}
