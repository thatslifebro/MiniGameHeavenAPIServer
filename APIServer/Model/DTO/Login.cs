﻿using System;
using System.ComponentModel.DataAnnotations;


namespace APIServer.Model.DTO;

public class LoginRequest
{
    [Required]
    public Int64 PlayerId { get; set; }

    [Required]

    public string Token { get; set; }
}

public class LoginResponse
{
    [Required] public ErrorCode Result { get; set; } = ErrorCode.None;
    [Required] public string Token { get; set; } = "";
}