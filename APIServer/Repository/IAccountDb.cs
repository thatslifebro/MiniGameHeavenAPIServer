﻿using System;
using System.Threading.Tasks;


namespace APIServer.Services;

public interface IAccountDb : IDisposable
{
    public Task<ErrorCode> CreateAccountAsync(Int64 playerId, string nickname);
    
    public Task<(ErrorCode, int)> VerifyUser(Int64 playerId);
}