﻿using APIServer.DTO.Ranking;
using APIServer.Models;
using CloudStructures.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIServer.Repository.Interfaces;

public interface IMemoryDb
{
    public Task<ErrorCode> RegistUserAsync(string token, int uid);
    public Task<ErrorCode> CheckUserAuthAsync(string id, string authToken);
    public Task<(bool, RdbAuthUserData)> GetUserAsync(string id);
    public Task<bool> SetUserStateAsync(RdbAuthUserData user, UserState userState);
    public Task<bool> SetUserReqLockAsync(string key);
    public Task<bool> DelUserReqLockAsync(string key);
    public Task<ErrorCode> DelUserAuthAsync(int uid);
    public  Task<ErrorCode> SetUserScore(int uid, int score);
    public  Task<ErrorCode> LoadUserScore();
    public  Task<(ErrorCode, List<RankData>)> GetTopRanking();
    public Task<(ErrorCode, Int64)> GetUserRankAsync(int uid);
}
