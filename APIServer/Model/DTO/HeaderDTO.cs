using Microsoft.AspNetCore.Mvc;

namespace APIServer.Model.DTO
{
    public class HeaderDTO
    {
        [FromHeader]
        public int Uid { get; set; }
    }
}
