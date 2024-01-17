using APIServer.Controllers.Game;
using APIServer.Model.DTO;
using APIServer.Model.DTO.DataLoad;
using APIServer.Servicies;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace APIServer.Controllers.DataLoad;

[ApiController]
[Route("[controller]")]
public class DataLoad : ControllerBase
{
    readonly ILogger<DataLoad> _logger;
    readonly IDataLoadService _dataLoadService;

    public DataLoad(ILogger<DataLoad> logger, IDataLoadService dataLoadService)
    {
        _logger = logger;
        _dataLoadService = dataLoadService;
    }

    /// <summary>
    /// 유저 기본 데이터 로드 API
    /// 게임에 필요한 정보(유저의 정보(점수,머니), 게임 정보, 친구 정보, 아이템 보유정보(캐릭터, 스킨, 코스튬, 푸드 및 푸드기어), 우편정보, 출석정보)을 조회합니다.
    /// </summary>

    [HttpGet]
    public async Task<DataLoadResponse> GetDataLoad([FromHeader] HeaderDTO header)
    {
        DataLoadResponse response = new();

        (response.Result, response.UserData) = await _dataLoadService.LoadUserData(header.Uid);

        return response;
    }
}
