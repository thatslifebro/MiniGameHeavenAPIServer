using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;
using static LogManager;

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

    /// <summary>
    /// 친구 요청을 보내거나 친구 요청을 수락하는 API </br>
    /// 상대 방의 친구 요청 유무에 따라 친구 수락 혹은 친구 요청 보내기를 수행합니다.
    /// </summary>
    [HttpPost]
    public async Task<FriendAddResponse> AddFriend(FriendAddRequest request)
    {
        FriendAddResponse response = new();
        if(request.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendAddFailSameUid;
            return response;
        }
        
        var errorCode = await _friendService.SendFriendReqOrAcceptReq(request.Uid, request.FriendUid);
        if(errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation(EventIdDic[EventType.FriendAdd], $"Uid : {request.Uid}");

        return response;

    }

}


