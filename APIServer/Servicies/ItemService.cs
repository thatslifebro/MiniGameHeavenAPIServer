using APIServer.Model.DAO.GameDB;
using APIServer.Model.DTO.DataLoad;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
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
    }
}
