using APIServer.Model.DTO.Friend;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// 받은 친구요청 리스트
[ApiController]
[Route("[controller]")]
public class FriendRequestList : ControllerBase
{
    readonly ILogger<FriendRequestList> _logger;
    readonly IAccountDb _accountDb;

    public FriendRequestList(ILogger<FriendRequestList> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost]
    public async Task<FriendRequestListResponse> GetFriendRequestList(FriendRequestListRequest request)
    {
        FriendRequestListResponse response = new();
        (response.Result, response.FriendRequestList) = await _accountDb.GetFriendRequestList(request.Uid);
        return response;
    }
}

