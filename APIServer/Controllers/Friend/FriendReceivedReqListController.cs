﻿using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LogManager;
using ZLogger;

// user_info에 역대 최고기록, 현재시즌기록, 이전시즌기록 추가해서 api에 3가지 옵션을 줄수 있게 변경 후 각 옵션별 orderby 정렬 필요.
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
    [HttpPost]
    public async Task<FriendRequestListResponse> GetFriendRequestList(FriendRequestListRequest request)
    {
        FriendRequestListResponse response = new();
        (response.Result, response.FriendRequestList) = await _friendService.GetFriendReceivedReqList(request.Uid);

        _logger.ZLogInformation(EventIdDic[EventType.FriendReceivedReqList], $"Uid : {request.Uid}");
        return response;
    }
}