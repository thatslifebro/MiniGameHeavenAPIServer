using APIServer.MasterData;
using APIServer.Model.DTO.DataLoad;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Servicies.Interfaces
{
    public interface IMailService
    {
        public Task<(ErrorCode, List<UserMailInfo>)> GetMailList(int uid);
        public Task<(ErrorCode, IEnumerable<RewardData>)> ReceiveMail(int uid, int mailSeq);
        public Task<ErrorCode> DeleteMail(int uid, int mailSeq);
    }
}
