using APIServer.Model.DAO.GameDB;
using APIServer.Model.DTO.DataLoad;
using System;
using System.Collections.Generic;

namespace APIServer.Model.DTO.Mail
{
    public class MailboxInfoResponse : ErrorCodeDTO
    {
        public List<UserMailInfo> MailInfo { get; set; }
    }

}
