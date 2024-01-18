using Microsoft.AspNetCore.Mvc;

namespace APIServer.Model.DTO.Mail
{
    
    public class MailReceiveRequest
    {
        public int MailSeq { get; set; }
    }

    public class MailReceiveResponse : ErrorCodeDTO
    {
    }
}
