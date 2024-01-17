using System.Threading.Tasks;
using System;
using APIServer.Model.DTO.Friend;
using System.Collections.Generic;
using APIServer.Model.DAO.GameDB;

namespace APIServer.Servicies.Interfaces;

public interface IFriendService
{
    public Task<ErrorCode> SendFriendReqOrAcceptReq(int uid, int friendUid);
    public Task<(ErrorCode, IEnumerable<FriendUserInfo>)> GetFriendList(int uid, string orderby = "bestscore_ever");
    public Task<(ErrorCode, IEnumerable<FriendReqListInfo>)> GetFriendReceivedReqList(int uid);
    public Task<(ErrorCode, IEnumerable<FriendReqListInfo>)> GetFriendSentReqList(int uid);
    public Task<ErrorCode> DeleteFriend(int uid, int friendUid);
    public Task<ErrorCode> DeleteFriendReq(int uid, int friendUid);
}
