﻿namespace APIServer.Model.DTO.Mail
{
    public class MailDeleteRequest
    {
        public int MailSeq { get; set; }
    }
    public class MailDeleteResponse : ErrorCodeDTO
    {
    }
}
