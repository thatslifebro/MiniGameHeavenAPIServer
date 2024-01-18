using System.Collections.Generic;
using System.Threading.Tasks;
using APIServer.Model.DAO.GameDB;
using SqlKata.Execution;

namespace APIServer.Services;

public partial class GameDb : IGameDb
{
    public async Task<IEnumerable<GdbMailboxInfo>> GetMailList(int uid)
    {
        return await _queryFactory.Query("mailbox").Where("uid", uid)
                                                .OrderBy("mail_seq")
                                                .GetAsync<GdbMailboxInfo>();
    }

    public async Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int uid, int mailSeq)
    {
        return await _queryFactory.Query("mailbox_reward").Where("mail_seq", mailSeq)
                                                .GetAsync<GdbMailboxRewardInfo>();
    }
}