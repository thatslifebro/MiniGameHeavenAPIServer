using APIServer.Model.DTO.DataLoad;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies
{
    public class MailService : IMailService
    {
        readonly ILogger<MailService> _logger;
        readonly IGameDb _gameDb;
        public MailService(ILogger<MailService> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        public async Task<(ErrorCode,List<UserMailInfo>)> GetMailList(int uid)
        {
            try
            {
                List<UserMailInfo> userMailInfoList = new();
                UserMailInfo userMailInfo = new();

                var mailList = await _gameDb.GetMailList(uid);
                foreach (var mail in mailList)
                {
                    userMailInfo.MailInfo = mail;
                    userMailInfo.MailItems = await _gameDb.GetMailRewardList(uid, mail.mail_seq);
                    userMailInfoList.Add(userMailInfo);
                }

                return (ErrorCode.None, userMailInfoList);
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                                       $"[Mail.GetMailList] ErrorCode: {ErrorCode.MailListFailException}, Uid: {uid}");
                return (ErrorCode.MailListFailException, null);
            }
        }
    }
}
