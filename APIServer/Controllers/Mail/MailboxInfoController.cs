using APIServer.DTO;
using APIServer.DTO.Mail;
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
    /// 메일함 정보 API
    /// 유저의 메일함 정보를 가져옵니다.
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
