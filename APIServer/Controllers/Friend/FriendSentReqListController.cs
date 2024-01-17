using APIServer.Model.DTO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

[ApiController]
[Route("[controller]")]
public class FriendSentReqList : ControllerBase
{
    readonly ILogger<FriendSentReqList> _logger;
    readonly IFriendService _friendService;

    public FriendSentReqList(ILogger<FriendSentReqList> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 보낸 친구 요청 조회 API
    /// 내가 받은 친구 요청 목록을 조회합니다.
    /// </summary>
    [HttpGet]
    public async Task<FriendRequestListResponse> GetFriendSentList([FromHeader] HeaderDTO header)
    {
        FriendRequestListResponse response = new();
        (response.Result, response.FriendRequestList) = await _friendService.GetFriendSentReqList(header.Uid);

        _logger.ZLogInformation($"[FriendSentReqList] Uid : {header.Uid}");
        return response;
    }
}