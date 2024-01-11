using APIServer.Model.DTO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace APIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendList : ControllerBase
{
    readonly ILogger<FriendList> _logger;
    readonly IAccountDb _accountDb;

    public FriendList(ILogger<FriendList> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost]
    public async Task<FriendListResponse> GetFriendList(FriendListRequest request)
    {
        FriendListResponse response = new();
        (response.Result, response.FriendList) = await _accountDb.GetFriendList(request.Uid);
        return response;
    } 
}
