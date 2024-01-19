using APIServer.Model.DTO.DataLoad;
using System;
using System.ComponentModel.DataAnnotations;


namespace APIServer.Model.DTO.Auth;

public class LoginRequest
{
    [Required]
    public long PlayerId { get; set; }

    [Required]
    public string HiveToken { get; set; }
    public string Nickname { get; set; }
}

public class LoginResponse
{
    [Required] public ErrorCode Result { get; set; } = ErrorCode.None;
    [Required] public string Token { get; set; } = "";
    [Required] public int Uid { get; set; } = 0;

    public UserDataLoadInfo userData { get; set; }
}