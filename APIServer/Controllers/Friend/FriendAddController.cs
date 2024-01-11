using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

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

    [HttpPost]
    public async Task<FriendAddResponse> AddFriend(FriendAddRequest request)
    {
        FriendAddResponse response = new();
        if(request.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendAddFailSameUid;
            return response;
        }
        response.Result = await _friendService.SendFriendReqOrAcceptReq(request.Uid, request.FriendUid);
        return response;

    }

}


