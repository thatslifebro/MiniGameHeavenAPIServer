﻿using APIServer.Model.DTO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

[ApiController]
[Route("[controller]")]
public class FriendReceivedReqList : ControllerBase
{
    readonly ILogger<FriendReceivedReqList> _logger;
    readonly IFriendService _friendService;

    public FriendReceivedReqList(ILogger<FriendReceivedReqList> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 받은 친구 요청 조회 API
    /// 내가 받은 친구 요청 목록을 조회합니다.
    /// </summary>
    [HttpGet]
    public async Task<FriendRequestListResponse> GetFriendRequestList([FromHeader] HeaderDTO header)
    {
        FriendRequestListResponse response = new();
        (response.Result, response.FriendRequestList) = await _friendService.GetFriendReceivedReqList(header.Uid);

        _logger.ZLogInformation($"[FriendReceivedReqList] Uid : {header.Uid}");
        return response;
    }
}