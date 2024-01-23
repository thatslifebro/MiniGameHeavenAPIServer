using APIServer.DTO;
using APIServer.DTO.Friend;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendCancelReq : ControllerBase
{
    readonly ILogger<FriendDelete> _logger;
    readonly IFriendService _friendService;

    public FriendCancelReq(ILogger<FriendDelete> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    //TODO 최흥배: 이미 Delete가 있는데 이것은 무엇인가요? 그리고 파일과 API 이름이 서로 다릅니다. 일치시켜주세요
    // 김성연 : 친구 삭제가 아닌 보낸 친구 요청의 취소 API입니다.

    /// <summary>
    /// 친구 요청 취소 API
    /// 보낸 친구 요청을 취소합니다.
    /// </summary>
    [HttpDelete]
    public async Task<FriendDeleteResponse> CancelFriendReq([FromHeader] HeaderDTO header, FriendDeleteRequest request)
    {
        FriendDeleteResponse response = new();
        if (header.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendDeleteFailSameUid;
            return response;
        }

        var errorCode = await _friendService.CancelFriendReq(header.Uid, request.FriendUid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[FriendCancelReq] Uid : {header.Uid}");
        return response;
    }
}
