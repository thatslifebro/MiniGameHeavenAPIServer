using APIServer.DTO;
using APIServer.DTO.Friend;
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

    //TODO 최흥배: 클라이언트를 만든다는 가정하에 이 API는 어떤 상황에서 호출 되는 것일까요?
    // 김성연 : 자신이 받은 친구 요청들을 볼 때 사용하는 API 입니다. 

    /// <summary>
    /// 받은 친구 요청 조회 API
    /// 내가 받은 친구 요청 목록을 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendRequestListResponse> GetFriendRequestList([FromHeader] HeaderDTO header)
    {
        FriendRequestListResponse response = new();

        (response.Result, response.FriendRequestList) = await _friendService.GetFriendReceivedReqList(header.Uid);

        _logger.ZLogInformation($"[FriendReceivedReqList] Uid : {header.Uid}");
        return response;
    }
}