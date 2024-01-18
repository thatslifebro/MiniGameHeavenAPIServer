using APIServer.Model.DTO;
using APIServer.Model.DTO.Mail;
using APIServer.Servicies.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using ZLogger;

namespace APIServer.Controllers.Mail;

[ApiController]
[Route("[controller]")]
public class MailboxInfo : ControllerBase
{
    readonly ILogger<MailboxInfo> _logger;
    readonly IMailService _mailService;

    public MailboxInfo(ILogger<MailboxInfo> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    /// <summary>
    /// 출석 정보 API </br>
    /// 유저의 출석 정보(누적 출석일, 최근 출석 일시)를 전달합니다.
    /// </summary>
    [HttpGet]
    public async Task<MailboxInfoResponse> GetMailboxInfo([FromHeader] HeaderDTO header)
    {
        MailboxInfoResponse response = new();

        (response.Result, response.MailInfo) = await _mailService.GetMailList(header.Uid);

        _logger.ZLogInformation($"[MailboxInfo] Uid : {header.Uid}");
        return response;
    }
}
