using APIServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIServer.DTO.Mail
{

    public class MailReceiveRequest
    {
        public int MailSeq { get; set; }
    }

    public class MailReceiveResponse : ErrorCodeDTO
    {
        public List<ReceivedReward> Rewards { get; set; }
    }

}
