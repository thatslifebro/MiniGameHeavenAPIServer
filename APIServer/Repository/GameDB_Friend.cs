using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using APIServer.Models.GameDB;
using APIServer.Repository.Interfaces;
using SqlKata.Execution;

namespace APIServer.Services;

public partial class GameDb : IGameDb
{
    public async Task<GdbFriendInfo> GetFriendReqInfo(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .FirstOrDefaultAsync<GdbFriendInfo>();
    }

    public async Task<int> InsertFriendReq(int uid, int friendUid, bool accept = false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    accept_yn = accept,
                                    create_dt = DateTime.Now,
                                });
    }

    public async Task<int> InsertFriendReq(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend")
                                .InsertAsync(new
                                {
                                    uid = uid,
                                    friend_uid = friendUid,
                                    accept_yn = accept,
                                    create_dt = DateTime.Now,
                                }, transaction);
    }

    public async Task<int> UpdateFriendReqAccept(int uid, int friendUid, IDbTransaction transaction, bool accept = false)
    {
        return await _queryFactory.Query("friend").Where("uid", friendUid).Where("friend_uid", uid).UpdateAsync(new
        {
            accept_yn = accept,
        }, transaction);
    }

    public async Task<IEnumerable<FriendUserInfo>> GetFriendUserInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", true)
                                .Select("user.uid", "nickname", "bestscore_ever", "recent_login_dt")
                                .GetAsync<FriendUserInfo>();
    }

    public async Task<IEnumerable<FriendReqListInfo>> GetFriendReceivedReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.uid")
                                .Where("friend.friend_uid", uid)
                                .Where("accept_yn", false)
                                .Select("user.uid", "user.nickname", "friend.create_dt")
                                .GetAsync<FriendReqListInfo>();
    }

    public async Task<IEnumerable<FriendReqListInfo>> GetFriendSentReqInfoList(int uid)
    {
        return await _queryFactory.Query("friend")
                                .Join("user", "user.uid", "friend.friend_uid")
                                .Where("friend.uid", uid)
                                .Where("accept_yn", false)
                                .Select("user.uid", "user.nickname", "friend.create_dt")
                                .GetAsync<FriendReqListInfo>();
    }

    public async Task<int> DeleteFriendEachOther(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .WhereRaw($"(uid={uid} AND friend_uid={friendUid}) OR (uid={friendUid} AND friend_uid={uid})")
                                .DeleteAsync();
    }

    public async Task<int> DeleteFriendReq(int uid, int friendUid)
    {
        return await _queryFactory.Query("friend")
                                .Where("uid", uid)
                                .Where("friend_uid", friendUid)
                                .DeleteAsync();
    }
}