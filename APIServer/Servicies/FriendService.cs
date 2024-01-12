﻿using APIServer.Model.DAO;
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
using System.Linq;

namespace APIServer.Servicies;

public class FriendService : IFriendService
{
    readonly ILogger<FriendService> _logger;
    readonly IAccountDb _accountDb;
    public FriendService(ILogger<FriendService> logger, IAccountDb accountDb)
    {
        _accountDb = accountDb;
        _logger = logger;
    }

    public async Task<ErrorCode> SendFriendReqOrAcceptReq(int uid, int friendUid)
    {
        try
        {
            AdbUserInfo userInfo = await _accountDb.GetUserByUid(friendUid);
            //없는 유저일 때
            if (userInfo is null)
            {
                _logger.ZLogDebug(
                $"[Friend.SendFriendReqOrAcceptReq] ErrorCode: {ErrorCode.FriendAddFailUserNotExist}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendAddFailUserNotExist;
            }
            //이미 친구신청 했을 때
            AdbFriendReqInfo friendReqInfo = await _accountDb.GetFriendReqInfo(uid, friendUid);
            if (friendReqInfo is not null)
            {
                _logger.ZLogDebug(
                $"[Friend.SendFriendReqOrAcceptReq] ErrorCode: {ErrorCode.FriendAddFailAlreadyFriend}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendAddFailAlreadyFriend;
            }
            //친구 요청
            friendReqInfo = await _accountDb.GetFriendReqInfo(friendUid, uid);
            var rowCount = 0;
            if (friendReqInfo is null)
            {
                rowCount = await _accountDb.InsertFriendReq(uid, friendUid);
                if(rowCount != 1)
                {
                    _logger.ZLogDebug(
                    $"[Friend.SendFriendReqOrAcceptReq] ErrorCode: {ErrorCode.FriendAddFailInsert}, Uid: {uid}, FriendUid : {friendUid}");
                    return ErrorCode.FriendAddFailInsert;
                }
                return ErrorCode.None;
            }
            //친구 요청 승낙
            return await AcceptFriendRequest(uid, friendUid);
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                $"[Friend.SendFriendReqOrAcceptReq] ErrorCode: {ErrorCode.FriendAddFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendAddFailException;
        }
    }

    public async Task<(ErrorCode, IEnumerable<AdbFriendUserInfo>)> GetFriendList(int uid, string orderby)
    {
        try
        {
            if(!new[] {"bestscore_ever", "bestscore_prev_season", "bestscore_cur_season" }.Contains(orderby))
            {
                _logger.ZLogDebug(
                $"[Friend.GetFriendList] ErrorCode: {ErrorCode.FriendGetListFailOrderby}, orderby: {orderby}");
                return (ErrorCode.FriendGetListFailOrderby, null);
            }
            return (ErrorCode.None, await _accountDb.GetFriendUserInfoList(uid, orderby));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Friend.GetFriendList] ErrorCode: {ErrorCode.FriendGetListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetListFailException, null);
        }
    }

    public async Task<(ErrorCode, IEnumerable<AdbFriendReqListInfo>)> GetFriendReceivedReqList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _accountDb.GetFriendReceivedReqInfoList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Friend.GetFriendList] ErrorCode: {ErrorCode.FriendGetRequestListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetRequestListFailException, null);
        }
    }

    public async Task<(ErrorCode, IEnumerable<AdbFriendReqListInfo>)> GetFriendSentReqList(int uid)
    {
        try
        {
            return (ErrorCode.None, await _accountDb.GetFriendSentReqInfoList(uid));
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,
                               $"[Friend.GetFriendSentReqList] ErrorCode: {ErrorCode.FriendGetRequestListFailException}, Uid: {uid}");
            return (ErrorCode.FriendGetRequestListFailException, null);
        }
    }

    public async Task<ErrorCode> DeleteFriend(int uid, int friendUid)
    {
        try
        {
            //친구가 아닐 때
            AdbFriendReqInfo friendInfo = await _accountDb.GetFriendReqInfo(uid, friendUid);
            if (friendInfo is null || friendInfo.accept_yn==false)
            {
                _logger.ZLogDebug(
                $"[Friend.DeleteFriend] ErrorCode: {ErrorCode.FriendDeleteFailNotFriend}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendDeleteFailNotFriend;
            }

            var rowCount = await _accountDb.DeleteFriendEachOther(uid, friendUid);
            if(rowCount != 2)
            {
                _logger.ZLogDebug(
                                   $"[Friend.DeleteFriend] ErrorCode: {ErrorCode.FriendDeleteFailDelete}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendDeleteFailDelete;
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[Friend.DeleteFriend] ErrorCode: {ErrorCode.FriendDeleteFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendDeleteFailException;
        }
    }

    public async Task<ErrorCode> DeleteFriendReq(int uid, int friendUid)
    {
        try
        {
            //친구 요청을 안했거나 친구 상태 일때
            AdbFriendReqInfo friendInfo = await _accountDb.GetFriendReqInfo(uid, friendUid);
            if (friendInfo is null || friendInfo.accept_yn == true)
            {
                _logger.ZLogDebug(
                $"[Friend.DeleteFriendReq] ErrorCode: {ErrorCode.FriendDeleteReqFailNotFriend}, Uid: {uid}, FriendUid : {friendUid}");
                return ErrorCode.FriendDeleteReqFailNotFriend;
            }

            var rowCount = await _accountDb.DeleteFriendReq(uid, friendUid);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.DeleteFriendReq] ErrorCode: {ErrorCode.FriendDeleteReqFailDelete}, Uid: {uid}, FriendUid : {friendUid}");
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"[Friend.DeleteFriendReq] ErrorCode: {ErrorCode.FriendDeleteReqFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendDeleteReqFailException;
        }
    }

    async Task<ErrorCode> AcceptFriendRequest(int uid, int friendUid)
    {
        var transaction = _accountDb.ADbConnection().BeginTransaction();
        try
        {
            var rowCount = await _accountDb.InsertFriendReq(uid, friendUid, transaction, true);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAddFailInsert}, Uid: {uid}, FriendUid : {friendUid}");
                transaction.Rollback();
                return ErrorCode.FriendAddFailInsert;
            }
            rowCount = await _accountDb.UpdateFriendReqAccept(uid, friendUid, transaction, true);
            if (rowCount != 1)
            {
                _logger.ZLogDebug(
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAddFailInsert}, Uid: {uid}, FriendUid : {friendUid}");
                transaction.Rollback();
                return ErrorCode.FriendAddFailInsert;
            }
            transaction.Commit();
            return ErrorCode.None;
        }
        catch(Exception e)
        {
            _logger.ZLogError(e,
                $"[Friend.AcceptFriendRequest] ErrorCode: {ErrorCode.FriendAcceptFailException}, Uid: {uid}, FriendUid : {friendUid}");
            return ErrorCode.FriendAcceptFailException;
        }
        finally
        {
            transaction.Dispose();
        }
    }

}