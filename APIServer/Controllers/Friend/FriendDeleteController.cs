using APIServer.Model.DTO.Friend;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class FriendDelete : ControllerBase
{
    readonly ILogger<FriendDelete> _logger;
    readonly IAccountDb _accountDb;

    public FriendDelete(ILogger<FriendDelete> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpDelete]
    public async Task<FriendDeleteResponse> DeleteFriend(FriendDeleteRequest request)
    {
        FriendDeleteResponse response = new();
        if (request.Uid == request.FriendUid)
        {
            response.Result = ErrorCode.FriendDeleteFailSameUid;
            return response;
        }
        response.Result = await _accountDb.DeleteFriend(request.Uid, request.FriendUid);
        return response;
    }
}