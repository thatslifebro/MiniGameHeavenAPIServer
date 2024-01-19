using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using APIServer.Servicies.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Servicies
{
    public class UserService : IUserService
    {
        readonly ILogger<UserService> _logger;
        readonly IGameDb _gameDb;
        public UserService(ILogger<UserService> logger, IGameDb gameDb)
        {
            _logger = logger;
            _gameDb = gameDb;
        }

        public async Task<(ErrorCode, GdbUserInfo)> GetUserInfo(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetUserByUid(uid));
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[User.GetUserInfo] ErrorCode: {ErrorCode.UserInfoFailException}, Uid: {uid}");
                return (ErrorCode.UserInfoFailException, null);
            }
        }

        public async Task<(ErrorCode, GdbUserMoneyInfo)> GetUserMoneyInfo(int uid)
        {
            try
            {
                return (ErrorCode.None, await _gameDb.GetUserMoneyById(uid));
            }
            catch (Exception e)
            {
                _logger.ZLogError(e,
                    $"[User.GetUserMoneyInfo] ErrorCode: {ErrorCode.UserMoneyInfoFailException}, Uid: {uid}");
                return (ErrorCode.UserMoneyInfoFailException, null);
            }
        }
    }
}
