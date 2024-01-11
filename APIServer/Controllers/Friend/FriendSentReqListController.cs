using APIServer.Model.DTO.Friend;
using APIServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

// 보낸 친구요청 리스트
// user_info에 역대 최고기록, 현재시즌기록, 이전시즌기록 추가해서 api에 3가지 옵션을 줄수 있게 변경 후 각 옵션별 orderby 정렬 필요.
[ApiController]
[Route("[controller]")]
public class FriendSentReqList : ControllerBase
{
    readonly ILogger<FriendSentReqList> _logger;
    readonly IAccountDb _accountDb;

    public FriendSentReqList(ILogger<FriendSentReqList> logger, IAccountDb accountDb)
    {
        _logger = logger;
        _accountDb = accountDb;
    }

    [HttpPost]
    public async Task<FriendRequestListResponse> GetFriendSentList(FriendRequestListRequest request)
    {
        FriendRequestListResponse response = new();
        (response.Result, response.FriendRequestList) = await _accountDb.GetFriendSentReqList(request.Uid);
        return response;
    }
}