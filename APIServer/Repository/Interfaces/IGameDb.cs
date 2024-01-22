﻿using APIServer.Models;
using APIServer.Models.GameDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace APIServer.Repository.Interfaces;

public interface IGameDb
{
    public Task<GdbUserInfo> GetUserByPlayerId(long playerId);
    public Task<GdbUserInfo> GetUserByUid(int uid);
    public Task<GdbUserInfo> GetUserByNickname(string nickname);
    public Task<int> InsertUser(long playerId, string nickname);
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
    public Task<int> UpdateUserBestScoreEver(int uid);
    public Task<int> UpdateRecentPlayDt(int uid, int gameKey);
    public Task<int> UpdateGamePlayChar(int uid, int gameKey, int charKey);
    public Task<int> InsertInitCharacter(int uid, IDbTransaction transaction);
    public Task<IEnumerable<GdbUserCharInfo>> GetCharList(int uid);
    public Task<IEnumerable<GdbUserCharRandomSkillInfo>> GetCharRandomSkillInfo(int uid, int charKey);
    public Task<GdbUserCharInfo> GetCharInfo(int uid, int charKey);
    public Task<int> InsertUserChar(int uid, int charKey, int cnt);
    public Task<int> LevelUpChar(int uid, int charKey, int level, int cnt);
    public Task<int> IncrementCharCnt(int uid, int charKey, int qty);
    public Task<int> SetCharCostume(int uid, int charKey, string costumeJsonString);
    public Task<IEnumerable<GdbUserSkinInfo>> GetSkinList(int uid);
    public Task<GdbUserSkinInfo> GetSkinInfo(int uid, int skinKey);
    public Task<int> InsertUserSkin(int uid, int skinKey);
    public Task<IEnumerable<GdbUserCostumeInfo>> GetCostumeList(int uid);
    public Task<GdbUserCostumeInfo> GetCostumeInfo(int uid, int costumeKey);
    public Task<int> InsertUserCostume(int uid, int costumeKey, int cnt);
    public Task<int> LevelUpCostume(int uid, int costumeKey, int level, int cnt);
    public Task<int> IncrementCostumeCnt(int uid, int costumeKey, int qty);
    public Task<IEnumerable<GdbUserFoodInfo>> GetFoodList(int uid);
    public Task<GdbUserFoodInfo> GetFoodInfo(int uid, int foodKey);
    public Task<int> InsertUserFood(int uid, int foodKey, int qty = 0, int gearQty = 0);
    public Task<int> IncrementFoodQty(int uid, int foodKey, int qty);
    public Task<int> IncrementFoodGearQty(int uid, int foodKey, int qty);
    public Task<int> FoodDecrement(int uid, int foodKey, int foodQty);
    public Task<IEnumerable<GdbMailboxInfo>> GetMailList(int uid);
    public Task<GdbMailboxInfo> GetMailInfo(int mailSeq);
    public Task<IEnumerable<GdbMailboxRewardInfo>> GetMailRewardList(int mailSeq);
    public Task<int> DeleteMail(int mailSeq);
    public Task<int> DeleteMailReward(int mailSeq);
    public Task<int> UpdateReceiveDt(int mailSeq);
    public Task<GdbAttendanceInfo> GetAttendanceById(int uid);
    public Task<GdbUserMoneyInfo> GetUserMoneyById(int uid);
    public Task<int> UpdateMainChar(int uid, int charKey);
    public Task<IEnumerable<RdbUserScoreData>> SelectAllUserScore();
    public Task<int> CheckAttendanceById(int uid);
    public Task<int> UpdateUserjewelry(int uid, int rewardQty);
    public IDbConnection GDbConnection();
}