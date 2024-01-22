using APIServer.DTO.Ranking;
using APIServer.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Ranking;

[ApiController]
[Route("[controller]")]
public class Ranking : ControllerBase
{
    readonly ILogger<Ranking> _logger;
    readonly IMemoryDb _memoryDb;

    public Ranking(ILogger<Ranking> logger, IMemoryDb memoryDb)
    {
        _logger = logger;
        _memoryDb = memoryDb;
    }

    /// <summary>
    /// 랭킹 조회 API
    /// 전체 랭킹을 조회합니다.
    /// </summary>
    [HttpGet]
    public async Task<RankingResponse> GetRanking()
    {
        RankingResponse response = new();
    
        (response.Result, response.RankingData) = await _memoryDb.GetUserRanking();

        _logger.ZLogInformation($"[Ranking] GetRanking");
        return response;
    }
}
