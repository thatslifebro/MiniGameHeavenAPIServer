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
public class MailReceive : ControllerBase
{
    readonly ILogger<MailReceive> _logger;
    readonly IMailService _mailService;

    public MailReceive(ILogger<MailReceive> logger, IMailService mailService)
    {
        _logger = logger;
        _mailService = mailService;
    }

    [HttpPost]
    public async Task<MailReceiveResponse> GetMailReceive([FromHeader] HeaderDTO header, MailReceiveRequest request)
    {
        MailReceiveResponse response = new();

        response.Result = await _mailService.ReceiveMail(header.Uid, request.MailSeq);

        _logger.ZLogInformation($"[MailboxInfo] Uid : {header.Uid}");
        return response;
    }
}
