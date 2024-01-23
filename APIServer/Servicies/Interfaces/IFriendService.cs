﻿using System.Threading.Tasks;
using System.Collections.Generic;
using APIServer.Models.GameDB;

namespace APIServer.Servicies.Interfaces;

public interface IFriendService
{
    public Task<ErrorCode> SendFriendReq(int uid, int friendUid);
    public Task<ErrorCode> AcceptFriendReq(int uid, int friendUid);
    public Task<(ErrorCode, IEnumerable<FriendUserInfo>)> GetFriendList(int uid);
    public Task<(ErrorCode, IEnumerable<FriendReqListInfo>)> GetFriendReceivedReqList(int uid);
    public Task<(ErrorCode, IEnumerable<FriendReqListInfo>)> GetFriendSentReqList(int uid);
    public Task<ErrorCode> DeleteFriend(int uid, int friendUid);
    public Task<ErrorCode> CancelFriendReq(int uid, int friendUid);
}
