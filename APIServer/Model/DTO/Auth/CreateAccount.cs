using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO.Auth;

public class CreateAccountReq
{
    [Required]
    public long PlayerId { get; set; }

    [Required]

    public string HiveToken { get; set; }
    [Required]
    public string NickName { get; set; }
}

public class CreateAccountRes : ErrorCodeDTO
{
}