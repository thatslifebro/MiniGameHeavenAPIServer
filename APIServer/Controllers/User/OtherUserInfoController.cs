using APIServer.DTO;
using APIServer.DTO.User;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.User;

[ApiController]
[Route("[controller]")]
public class OtherUserInfo : ControllerBase
{
    readonly ILogger<OtherUserInfo> _logger;
    readonly IUserService _userService;

    public OtherUserInfo(ILogger<OtherUserInfo> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    /// <summary>
    /// 다른 유저 조회 API
    /// 다른 유저의 정보를 uid를 통해 조회합니다.
    /// </summary>

    [HttpPost]
    public async Task<UserInfoResponse> GetOtherUserInfo(UserInfoRequest request)
    {
        UserInfoResponse response = new();

        (response.Result, response.UserInfo) = await _userService.GetOtherUserInfo(request.Uid);

        _logger.ZLogInformation($"[UserInfo] Uid : {request.Uid}");
        return response;
    }
}
