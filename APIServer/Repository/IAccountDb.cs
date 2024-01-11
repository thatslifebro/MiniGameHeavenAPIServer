using APIServer.Model.DTO.Friend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIServer.Model.DAO;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<AdbUserInfo> GetUserByPlayerId(Int64 playerId);
    public Task<AdbUserInfo> GetUserByNickname(string nickname);
    public Task<int> InsertUser(Int64 playerId, string nickname);
    public Task<int> UpdateRecentLogin(int uid);
    public Task<ErrorCode> AddFriendByUid(int uid, int friendUid);
    public Task<(ErrorCode, List<FriendInfo>)> GetFriendList(int uid);
    public Task<(ErrorCode, List<FriendRequestInfo>)> GetFriendReceivedReqList(int uid);
    public Task<(ErrorCode, List<FriendRequestInfo>)> GetFriendSentReqList(int uid);
    public Task<ErrorCode> DeleteFriend(int uid, int friendUid);

}