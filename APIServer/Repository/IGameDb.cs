using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IGameDb
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
    public Task<IEnumerable<GdbGameInfo>> GetGameList(int uid);
    public Task<int> InsertInitGameList(int uid, IDbTransaction transaction);
    public Task<int> InsertGame(int uid, int gameKey);
    public Task<GdbGameInfo> GetGameInfo(int uid, int gameKey);
    public Task<int> UpdateBestscore(int uid, int gameKey, int score);
    public Task<int> UpdateBestscoreCurSeason(int uid, int gameKey, int score);
    public Task<int> UpdateRecentPlayDt(int uid, int gameKey);
    public Task<int> InsertInitCharacter(int uid, IDbTransaction transaction);
    public IDbConnection GDbConnection();
}