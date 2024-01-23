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
public class FriendAdd : ControllerBase
{
    readonly ILogger<FriendAdd> _logger;
    readonly IFriendService _friendService;

    public FriendAdd(ILogger<FriendAdd> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    //TODO 최흥배: 기능은 단순하게 하는 것이 좋습니다. 친구 요청과 요청 수락은 나누도록 하시죠 

    /// <summary>
    /// 친구 요청 API </br>
    /// 상대방에게 친구 요청을 보냅니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendAddResponse> AddFriend([FromHeader] HeaderDTO header, FriendAddRequest request)
    {
        FriendAddResponse response = new();
        if(header.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendAddFailSameUid;
            return response;
        }
        
        var errorCode = await _friendService.SendFriendReq(header.Uid, request.FriendUid);
        if(errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[FriendAdd] Uid : {header.Uid}");
        return response;

    }

}


