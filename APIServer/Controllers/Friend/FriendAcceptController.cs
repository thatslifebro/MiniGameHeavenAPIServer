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
public class FriendAccept : ControllerBase
{
    readonly ILogger<FriendAccept> _logger;
    readonly IFriendService _friendService;

    public FriendAccept(ILogger<FriendAccept> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 친구 요청을 수락하는 API </br>
    /// 친구 요청을 수락합니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendAcceptResponse> AcceptFriend([FromHeader] HeaderDTO header, FriendAcceptRequest request)
    {
        FriendAcceptResponse response = new();
        if(header.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendAcceptFailSameUid;
            return response;
        }
        
        var errorCode = await _friendService.AcceptFriendReq(header.Uid, request.FriendUid);
        if(errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[FriendAccept] Uid : {header.Uid}");
        return response;
    }
}
