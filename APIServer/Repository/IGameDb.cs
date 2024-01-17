﻿using APIServer.Model.DAO.GameDB;
using APIServer.Model.DTO.Friend;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace APIServer.Services;

public interface IGameDb
{
    public Task<GdbUserInfo> GetUserByPlayerId(Int64 playerId);
    public Task<GdbUserInfo> GetUserByUid(int uid);
    public Task<GdbUserInfo> GetUserByNickname(string nickname);
    public Task<int> InsertUser(Int64 playerId, string nickname);
    public Task<int> DeleteAccount(int uid);
    public Task<int> UpdateRecentLogin(int uid);
    public Task<GdbFriendInfo> GetFriendReqInfo(int uid, int friendUid);
    public Task<int> InsertFriendReq(int uid, int friendUid, bool accept = false);
    public Task<int> InsertFriendReq(int uid, int friendUid, IDbTransaction transaction, bool accept = false);
    public Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false);
    public Task<IEnumerable<FriendUserInfo>> GetFriendUserInfoList(int uid, string orderby);
    public Task<IEnumerable<FriendReqListInfo>> GetFriendReceivedReqInfoList(int uid);
    public Task<IEnumerable<FriendReqListInfo>> GetFriendSentReqInfoList(int uid);
    public Task<int> DeleteFriendEachOther(int uid, int friendUid);
    public Task<int> DeleteFriendReq(int uid, int friendUid);
    public Task<IEnumerable<GdbGameInfo>> GetGameList(int uid);
    public Task<int> InsertInitGameList(int uid, IDbTransaction transaction);
    public Task<int> InsertInitMoneyInfo(int uid, IDbTransaction transaction);
    public Task<int> InsertInitAttendance(int uid, IDbTransaction transaction);
    public Task<int> InsertGame(int uid, int gameKey);
    public Task<GdbGameInfo> GetGameInfo(int uid, int gameKey);
    public Task<int> UpdateBestscore(int uid, int gameKey, int score);
    public Task<int> UpdateBestscoreCurSeason(int uid, int gameKey, int score);
    public Task<int> UpdateRecentPlayDt(int uid, int gameKey);
    public Task<int> InsertInitCharacter(int uid, IDbTransaction transaction);
    public Task<IEnumerable<GdbUserCharInfo>> GetCharList(int uid);
    public Task<IEnumerable<GdbUserCharRandomSkillInfo>> GetCharRandomSkillInfo(int uid, int charKey);
    public Task<IEnumerable<GdbUserSkinInfo>> GetSkinList(int uid);
    public Task<IEnumerable<GdbUserCostumeInfo>> GetCostumeList(int uid);
    public Task<IEnumerable<GdbUserFoodInfo>> GetFoodList(int uid);
    public Task<IEnumerable<GdbMailboxInfo>> GetMailList(int uid);
    public Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int uid, int mailKey);
    public Task<GdbAttendanceInfo> GetAttendanceById(int uid);
    public Task<GdbUserMoneyInfo> GetUserMoneyById(int uid);
    public IDbConnection GDbConnection();
}