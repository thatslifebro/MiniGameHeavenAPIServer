using APIServer.Model.DTO.Friend;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace APIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendAdd : ControllerBase
{
    readonly ILogger<FriendAdd> _logger;
    readonly IAccountDb _accountDb;

    public FriendAdd(ILogger<FriendAdd> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
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
        response.Result = await _accountDb.AddFriendByUid(request.Uid, request.FriendUid);
        return response;

    }

}


