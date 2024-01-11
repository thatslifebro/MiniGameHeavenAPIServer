using APIServer.Model.DTO.Friend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname);
    
    public Task<(ErrorCode, int)> VerifyUser(Int64 playerId);
    public Task<ErrorCode> UpdateLastLoginTime(int uid);
    public Task<ErrorCode> AddFriendByUid(int uid, int friendUid);
    public Task<(ErrorCode, List<FriendInfo>)> GetFriendList(int uid);
    public Task<(ErrorCode, List<FriendRequestInfo>)> GetFriendReceivedReqList(int uid);
    public Task<(ErrorCode, List<FriendRequestInfo>)> GetFriendSentReqList(int uid);
    public Task<ErrorCode> DeleteFriend(int uid, int friendUid);

}