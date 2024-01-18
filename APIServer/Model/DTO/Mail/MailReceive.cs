using APIServer.MasterData;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace APIServer.Model.DTO.Mail
{
    
    public class MailReceiveRequest
    {
        public int MailSeq { get; set; }
    }

    public class MailReceiveResponse : ErrorCodeDTO
    {
        public IEnumerable<RewardData> Rewards { get; set; }
    }
}
