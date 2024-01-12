﻿using APIServer.Model.DAO;
using APIServer.Model.DTO.Friend;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IGameDb
{
    public Task<IEnumerable<GdbGameListInfo>> GetGameList(int uid);
    public Task<int> InsertInitGameList(int uid);
    public Task<int> InsertGame(int uid, int gameId);
    public Task<GdbGameInfo> GetGameInfo(int uid, int gameId);
}