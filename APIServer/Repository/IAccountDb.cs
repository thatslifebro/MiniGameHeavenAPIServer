using APIServer.Model.DTO.Friend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using APIServer.Model.DAO;
using System.Data;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public IDbConnection ADbConnection();
    public Task<AdbUserInfo> GetUserByPlayerId(Int64 playerId);
    public Task<AdbUserInfo> GetUserByUid(int uid);
    public Task<AdbUserInfo> GetUserByNickname(string nickname);
    public Task<int> InsertUser(Int64 playerId, string nickname);
    public Task<int> DeleteAccount(int uid);
    public Task<int> UpdateRecentLogin(int uid);
    public Task<AdbFriendReqInfo> GetFriendReqInfo(int uid, int friendUid);
    public Task<int> InsertFriendReq(int uid, int friendUid, bool accept = false);
    public Task<int> InsertFriendReq(int uid, int friendUid, IDbTransaction transaction, bool accept = false);
    public Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false);
    public Task<IEnumerable<AdbFriendUserInfo>> GetFriendUserInfoList(int uid, string orderby);
    public Task<IEnumerable<AdbFriendReqListInfo>> GetFriendReceivedReqInfoList(int uid);
    public Task<IEnumerable<AdbFriendReqListInfo>> GetFriendSentReqInfoList(int uid);
    public Task<int> DeleteFriendEachOther(int uid, int friendUid);
    public Task<int> DeleteFriendReq(int uid, int friendUid);

}