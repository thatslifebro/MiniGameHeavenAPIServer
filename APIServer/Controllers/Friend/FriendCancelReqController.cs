using APIServer.Model.DTO.Friend;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LogManager;
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

    [HttpDelete]
    public async Task<FriendDeleteResponse> DeleteFriendReq(FriendDeleteRequest request)
    {
        FriendDeleteResponse response = new();
        if (request.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendDeleteFailSameUid;
            return response;
        }
        var errorCode = await _friendService.DeleteFriendReq(request.Uid, request.FriendUid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation(EventIdDic[EventType.FriendCancelReq], $"Uid : {request.Uid}");
        return response;
    }
}
