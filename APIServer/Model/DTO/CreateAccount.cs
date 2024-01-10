using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO;

public class CreateAccountReq
{
    [Required]
    public Int64 PlayerId { get; set; }

    [Required]

    public string HiveToken { get; set; }
    [Required]
    public string NickName { get; set; }
}

public class CreateAccountRes : ErrorCodeDTO
{
}