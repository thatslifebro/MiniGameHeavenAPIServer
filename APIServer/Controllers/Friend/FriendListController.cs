using APIServer.DTO;
using APIServer.DTO.Friend;
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

    //TODO 최흥배: 사용 편이성을 위해 Get 방식은 사용하지 말고, 모두 Post 방식으로 사용하는 것이 좋습니다
    // 요청과 응답은 모두 Json 형식으로 주고 받는 것이 좋습니다

    /// <summary>
    /// 친구 목록 조회 API
    /// 친구 목록과 친구 정보를 불러옵니다.
    /// orderby에 따라서 역대 점수, 현재 시즌 점수, 이전 시즌 점수 순으로 정렬가능 합니다.
    /// </summary>
    [HttpGet]
    public async Task<FriendListResponse> GetFriendList([FromHeader] HeaderDTO header, [FromQuery(Name = "orderby")] string orderby = "bestscore_ever")
    {
        FriendListResponse response = new();

        //TODO 최흥배: 성능을 위해서 데이터를 가져올 때 정렬은 하지 않는 것이 좋습니다. 정렬은 클라이언트에서 하도록 합니다.
        (response.Result, response.FriendList) = await _friendService.GetFriendList(header.Uid, orderby);

        _logger.ZLogInformation($"[FriendList] Uid : {header.Uid}");
        return response;
    } 
}
