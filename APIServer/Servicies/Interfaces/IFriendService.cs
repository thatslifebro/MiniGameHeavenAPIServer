using System.Threading.Tasks;
using System;
using APIServer.Model.DTO.Friend;
using System.Collections.Generic;

namespace APIServer.Servicies.Interfaces;

public interface IFriendService
{
    public Task<ErrorCode> SendFriendReqOrAcceptReq(int uid, int friendUid);
    public Task<(ErrorCode, IEnumerable<FriendUserInfo>)> GetFriendList(int uid, string orderby);
    public Task<(ErrorCode, IEnumerable<FriendReqInfo>)> GetFriendReceivedReqList(int uid);
    public Task<(ErrorCode, IEnumerable<FriendReqInfo>)> GetFriendSentReqList(int uid);
    public Task<ErrorCode> DeleteFriend(int uid, int friendUid);
    public Task<ErrorCode> DeleteFriendReq(int uid, int friendUid);
}
