using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

[ApiController]
[Route("[controller]")]
public class FriendDelete : ControllerBase
{
    readonly ILogger<FriendDelete> _logger;
    readonly IFriendService _friendService;

    public FriendDelete(ILogger<FriendDelete> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    /// <summary>
    /// 친구를 삭제하는 API
    /// 서로 친구를 삭제합니다.
    /// </summary>
    [HttpDelete]
    public async Task<FriendDeleteResponse> DeleteFriend(FriendDeleteRequest request)
    {
        FriendDeleteResponse response = new();
        if (request.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendDeleteFailSameUid;
            return response;
        }
        var errorCode = await _friendService.DeleteFriend(request.Uid, request.FriendUid);
        if (errorCode != ErrorCode.None)
        {
            response.Result = errorCode;
            return response;
        }

        _logger.ZLogInformation($"[FriendDelete] Uid : {request.Uid}");
        return response;
    }
}