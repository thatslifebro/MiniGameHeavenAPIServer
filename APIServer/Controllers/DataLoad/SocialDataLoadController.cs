﻿using APIServer.DTO;
using APIServer.DTO.DataLoad;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.DataLoad;

[ApiController]
[Route("[controller]")]
public class SocialDataLoad : ControllerBase
{
    readonly ILogger<SocialDataLoad> _logger;
    readonly IDataLoadService _dataLoadService;

    public SocialDataLoad(ILogger<SocialDataLoad> logger, IDataLoadService dataLoadService)
    {
        _logger = logger;
        _dataLoadService = dataLoadService;
    }

    /// <summary>
    /// 소셜 데이터 로드 API
    /// 게임에 필요한 소셜 정보(친구 정보, 메일 정보)를 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<SocialDataLoadResponse> LoadSocialData([FromHeader] HeaderDTO header)
    {
        SocialDataLoadResponse response = new();

        (response.Result, response.SocialData) = await _dataLoadService.LoadSocialData(header.Uid);

        _logger.ZLogInformation($"[DataLoad] Uid : {header.Uid}");
        return response;
    }
}
