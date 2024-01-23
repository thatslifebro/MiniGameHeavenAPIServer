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
public class UserDataLoad : ControllerBase
{
    readonly ILogger<UserDataLoad> _logger;
    readonly IDataLoadService _dataLoadService;

    public UserDataLoad(ILogger<UserDataLoad> logger, IDataLoadService dataLoadService)
    {
        _logger = logger;
        _dataLoadService = dataLoadService;
    }

    /// <summary>
    /// 유저 데이터 로드 API
    /// 게임에 필요한 유저 정보(유저의 정보(점수,재화), 출석 정보)를 조회합니다.
    /// </summary>
    [HttpPost]
    public async Task<UserDataLoadResponse> LoadUserData([FromHeader] HeaderDTO header)
    {
        UserDataLoadResponse response = new();

        (response.Result, response.UserData) = await _dataLoadService.LoadUserData(header.Uid);

        _logger.ZLogInformation($"[DataLoad] Uid : {header.Uid}");
        return response;
    }
}
