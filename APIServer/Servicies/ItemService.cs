using APIServer.MasterData;
using APIServer.Model.DAO.GameDB;
using APIServer.Model.DTO.DataLoad;
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
        public ItemService(ILogger<ItemService> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

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
            try
            {
                var rowCount = 0;
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
                        //GetChar Service
                        //캐릭터가 없다면 insert, 있다면 캐릭터 개수 늘려주고 레벨업.
                        break;
                    case "skin": //스킨
                        //GetSkin Service
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
