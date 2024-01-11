using APIServer.Model.DAO;
using APIServer.Repository;
using APIServer.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using ZLogger;
using System.Data;
using APIServer.Controllers.Friend;
using System.Security.Cryptography;
using APIServer.Model.DTO.Friend;
using System.Collections.Generic;
using APIServer.Servicies.Interfaces;

namespace APIServer.Servicies;

public class FriendService : IFriendService
{
    readonly ILogger<FriendService> _logger;
    readonly IAccountDb _accountDb;
    readonly IMemoryDb _memoryDb;
    string _hiveServerAddress;
    public FriendService(ILogger<FriendService> logger, IAccountDb accountDb, IMemoryDb memoryDb)
    {
        _memoryDb = memoryDb;
        _accountDb = accountDb;
        _logger = logger;
    }

    public async Task<ErrorCode> SendFriendReqOrAcceptReq(int uid, int friendUid)
    {
        try
        {
            AdbUserInfo userInfo = await _accountDb.GetUserByUid(friendUid);
            //없는 유저라면 에러
            if (userInfo is null)
            {
                return ErrorCode.FriendAddFailUserNotExist;
            }
            //이미 친구신청 했다면 에러
            AdbFriendReqInfo friendReqInfo = await _accountDb.GetFriendReqInfo(uid, friendUid);
            if (friendReqInfo is not null)
            {
                return ErrorCode.FriendAddFailAlreadyFriend;
            }
            //친구 요청
            friendReqInfo = await _accountDb.GetFriendReqInfo(friendUid, uid);
            int count = 0;
            if (friendReqInfo is null)
            {
                count = await _accountDb.InsertFriendReq(uid, friendUid);
                return count != 1 ? ErrorCode.FriendAddFailInsert : ErrorCode.None;
            }
            //친구 요청 승낙
            return await AcceptFriendRequest(uid, friendUid);
        }
        catch (Exception e)
        {
            _logger.ZLogDebug(
                $"[AccountDb.Login] ErrorCode: {ErrorCode.FriendAddFailException}, Uid: {uid}, e :{e.Message}");
            return ErrorCode.FriendAddFailException;
        }

    }

    public async Task<(ErrorCode, IEnumerable<FriendUserInfo>)> GetFriendList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _accountDb.GetFriendUserInfoList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendGetListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetListFailException, null);
        }
    }

    public async Task<(ErrorCode, IEnumerable<FriendReqInfo>)> GetFriendReceivedReqList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _accountDb.GetFriendReceivedReqInfoList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendGetRequestListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetRequestListFailException, null);
        }
    }

    public async Task<(ErrorCode, IEnumerable<FriendReqInfo>)> GetFriendSentReqList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _accountDb.GetFriendSentReqInfoList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendGetRequestListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetRequestListFailException, null);
        }
    }

    public async Task<ErrorCode> DeleteFriend(int uid, int friendUid)
    {
        try
        {
            //친구가 아니라면
            AdbFriendReqInfo friendInfo = await _accountDb.GetFriendReqInfo(uid, friendUid);
            if (friendInfo is null || friendInfo.accept_yn==false)
            {
                return ErrorCode.FriendDeleteFailNotFriend;
            }

            int count = await _accountDb.DeleteFriendEachOther(uid, friendUid);
            return count != 2 ? ErrorCode.FriendDeleteFailDelete : ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendDeleteFailException}, Uid: {uid}");
            return ErrorCode.FriendDeleteFailException;
        }
    }

    public async Task<ErrorCode> DeleteFriendReq(int uid, int friendUid)
    {
        try
        {
            //이미 친구라면
            AdbFriendReqInfo friendInfo = await _accountDb.GetFriendReqInfo(uid, friendUid);
            if (friendInfo is null || friendInfo.accept_yn == true)
            {
                return ErrorCode.FriendDeleteFailNotFriend;
            }

            int count = await _accountDb.DeleteFriendReq(uid, friendUid);
            return count != 1 ? ErrorCode.FriendDeleteFailDelete : ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[AccountDb.GetFriendList] ErrorCode: {ErrorCode.FriendDeleteReqFailException}, Uid: {uid}");
            return ErrorCode.FriendDeleteReqFailException;
        }
    }

    async Task<ErrorCode> AcceptFriendRequest(int uid, int friendUid)
    {
        var transaction = _accountDb.ADbConnection().BeginTransaction();
        try
        {
            int count = await _accountDb.InsertFriendReq(uid, friendUid, transaction, true);
            if (count != 1)
            {
                transaction.Rollback();
                return ErrorCode.FriendAddFailInsert;
            }
            count = await _accountDb.UpdateFriendReqAccept(uid, friendUid, transaction, true);
            if (count != 1)
            {
                transaction.Rollback();
                return ErrorCode.FriendAddFailInsert;
            }
            transaction.Commit();
            return ErrorCode.None;
        }
        catch(Exception e)
        {
            _logger.ZLogDebug(
                $"[AccountDb.Login] ErrorCode: {ErrorCode.FriendAddFailException}, Uid: {uid}, e :{e.Message}");
            return ErrorCode.FriendAddFailException;
        }
        finally
        {
            transaction.Dispose();
        }
    }

}
