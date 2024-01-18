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
public class MailDelete : ControllerBase
{
    readonly ILogger<MailDelete> _logger;
    readonly IMailService _mailService;

    public MailDelete(ILogger<MailDelete> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    /// <summary>
    /// 메일 삭제 API
    /// 메일함에서 메일을 삭제합니다.
    /// </summary>
    [HttpDelete]
    public async Task<MailDeleteResponse> GetMailDelete([FromHeader] HeaderDTO header, MailDeleteRequest request)
    {
        MailDeleteResponse response = new();

        response.Result = await _mailService.DeleteMail(header.Uid, request.MailSeq);

        _logger.ZLogInformation($"[MailboxInfo] Uid : {header.Uid}");
        return response;
    }
}
