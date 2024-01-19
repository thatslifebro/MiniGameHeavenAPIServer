namespace APIServer.DTO.Game
{
    public class GameSetPlayCharRequest
    {
        public int GameKey { get; set; }
        public int CharKey { get; set; }
    }

    public class GameSetPlayCharResponse : ErrorCodeDTO
    {
    }
}
