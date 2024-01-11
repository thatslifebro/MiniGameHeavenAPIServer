using APIServer.Model.DTO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static LogManager;
using ZLogger;

namespace APIServer.Controllers.Friend;

[ApiController]
[Route("[controller]")]
public class FriendList : ControllerBase
{
    readonly ILogger<FriendList> _logger;
    readonly IFriendService _friendService;

    public FriendList(ILogger<FriendList> logger, IFriendService friendService)
    {
        _logger = logger;
        _friendService = friendService;
    }

    [HttpPost]
    public async Task<FriendListResponse> GetFriendList(FriendListRequest request)
    {
        FriendListResponse response = new();
        (response.Result, response.FriendList) = await _friendService.GetFriendList(request.Uid);

        _logger.ZLogInformation(EventIdDic[EventType.FriendList], $"Uid : {request.Uid}");
        return response;
    } 
}
