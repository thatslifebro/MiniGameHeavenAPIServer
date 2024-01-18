using APIServer.MasterData;
using APIServer.Model.DAO.GameDB;
using APIServer.Model.DTO.DataLoad;
using APIServer.Repository;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies
{
    public class ItemService : IItemService
    {
        readonly ILogger<ItemService> _logger;
        readonly IGameDb _gameDb;
        readonly IMasterDb _masterDb;
        public ItemService(ILogger<ItemService> logger, IGameDb gameDb, IMasterDb masterDb)
        {
            _logger = logger;
            _gameDb = gameDb;
            _masterDb = masterDb;
        }

        #region Character

        public async Task<(ErrorCode,List<UserCharInfo>)> GetCharList(int uid)
        {
            try
            {
                List<UserCharInfo> userCharInfoList = new();
                UserCharInfo userCharInfo = new();

                var charList =  await _gameDb.GetCharList(uid);
                foreach (var character in charList)
                {
                    userCharInfo.CharInfo = character;
                    userCharInfo.RandomSkills = await _gameDb.GetCharRandomSkillInfo(uid, character.char_key);
                    userCharInfoList.Add(userCharInfo);
                }

                return (ErrorCode.None, userCharInfoList);
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                    $"[Item.GetCharList] ErrorCode: {ErrorCode.CharListFailException}, Uid: {uid}");
                return (ErrorCode.CharListFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveChar(int uid, int charKey)
        {
            try
            {
                var charInfo = await _gameDb.GetCharInfo(uid, charKey);

                // 캐릭터가 없다면 추가
                if (charInfo == null)
                {
                    var rowCount = await _gameDb.InsertUserChar(uid, charKey);
                    if (rowCount != 1)
                    {
                        return ErrorCode.CharReceiveFailInsert;
                    }
                }
                // 있다면 수량증가 or 레벨 업
                else
                {
                    var levelUpCnt = _masterDb._itemLevelList.Find(data => data.level == charInfo.char_level).item_cnt;
                    // 레벨업
                    if (levelUpCnt == charInfo.char_cnt+1)
                    {
                        var rowCount = await _gameDb.LevelUpChar(uid, charKey);
                        if (rowCount != 1)
                        {
                            return ErrorCode.CharReceiveFailLevelUP;
                        }
                    }
                    // 수량증가
                    else
                    {
                        var rowCount = await _gameDb.IncrementCharCnt(uid, charKey);
                        if (rowCount != 1)
                        {
                            return ErrorCode.CharReceiveFailIncrementCharCnt;
                        }
                    }
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                                  $"[Item.ReceiveChar] ErrorCode: {ErrorCode.CharReceiveFailException}, Uid: {uid}, CharKey: {charKey}");
                return ErrorCode.CharReceiveFailException;
            }
        }

        #endregion

        #region Skin

        public async Task<(ErrorCode, IEnumerable<GdbUserSkinInfo>)> GetSkinList(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetSkinList(uid));
            }
            catch(System.Exception e)
            {
                _logger.ZLogError(e,
                                  $"[Item.GetSkinList] ErrorCode: {ErrorCode.SkinListFailException}, Uid: {uid}");
                return (ErrorCode.SkinListFailException, null);
            }
        }

        public async Task<ErrorCode> ReceiveSkin(int uid, int skinKey)
        {
            try
            {
                var skinInfo = await _gameDb.GetSkinInfo(uid, skinKey);

                // 스킨이 없다면 추가
                if (skinInfo != null)
                {
                    return ErrorCode.SkinReceiveFailAlreadyOwn;
                }

                var rowCount = await _gameDb.InsertUserSkin(uid, skinKey);
                if (rowCount != 1)
                {
                    return ErrorCode.SkinReceiveFailInsert;
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                                  $"[Item.ReceiveSkin] ErrorCode: {ErrorCode.SkinReceiveFailException}, Uid: {uid}, SkinKey: {skinKey}");
                return ErrorCode.SkinReceiveFailException;
            }
        }

        #endregion

        public async Task<(ErrorCode,IEnumerable<GdbUserCostumeInfo>)> GetCostumeList(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetCostumeList(uid));
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                                       $"[Item.GetCostumeList] ErrorCode: {ErrorCode.CostumeListFailException}, Uid: {uid}");
                return (ErrorCode.CostumeListFailException, null);
            }
        }

        public async Task<(ErrorCode,IEnumerable<GdbUserFoodInfo>)> GetFoodList(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetFoodList(uid));
            }
            catch (System.Exception e)
            {
                _logger.ZLogError(e,
                                                          $"[Item.GetFoodList] ErrorCode: {ErrorCode.FoodListFailException}, Uid: {uid}");
                return (ErrorCode.FoodListFailException, null);
            }
        }

        public async Task<ErrorCode> GetReward(int uid, RewardData reward)
        {
            int rowCount;
            ErrorCode errorCode;
            try
            {
                switch (reward.reward_type)
                {
                    case "money": //보석
                        rowCount = await _gameDb.UpdateUserjewelry(uid, reward.reward_qty);
                        if (rowCount != 1)
                        {
                            return ErrorCode.UserUpdateJewelryFailIncremnet;
                        }
                        break;
                    case "char": //캐릭터
                        errorCode = await ReceiveChar(uid, reward.reward_key);
                        break;
                    case "skin": //스킨
                        errorCode = await ReceiveSkin(uid, reward.reward_key);
                        break;
                    case "costume": //코스튬
                        //GetCostume Service
                        break;
                    case "food": //푸드
                        //GetFood Service
                        break;
                    case "gacha": // 가챠
                        //GetGacha Service
                        break;
                }

                return ErrorCode.None;
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[GetReward] ErrorCode: {ErrorCode.GetRewardFailException}, Uid: {uid}");
                return ErrorCode.GetRewardFailException;
            }
        }
    }
}
