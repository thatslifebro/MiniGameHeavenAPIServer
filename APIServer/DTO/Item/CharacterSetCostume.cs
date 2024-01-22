using APIServer.Models;
using APIServer.Models.GameDB;

namespace APIServer.DTO.Item
{
    public class CharacterSetCostumeRequest
    {
        public int CharKey { get; set; }
        public CharCostumeInfo CostumeInfo { get; set; }
    }

    public class CharacterSetCostumeResponse : ErrorCodeDTO
    {
    }
}
