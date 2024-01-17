using APIServer.Model.DTO;
using APIServer.Model.DTO.Friend;
using APIServer.Services;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
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

    /// <summary>
    /// 친구 목록 조회 API
    /// 친구 목록과 친구 정보를 불러옵니다.
    /// orderby에 따라서 역대 점수, 현재 시즌 점수, 이전 시즌 점수 순으로 정렬가능 합니다.
    /// </summary>
    [HttpGet]
    public async Task<FriendListResponse> GetFriendList([FromHeader] HeaderDTO header, [FromQuery(Name = "orderby")] string orderby = "bestscore_ever")
    {
        FriendListResponse response = new();
        (response.Result, response.FriendList) = await _friendService.GetFriendList(header.Uid, orderby);

        _logger.ZLogInformation($"[FriendList] Uid : {header.Uid}");
        return response;
    } 
}
