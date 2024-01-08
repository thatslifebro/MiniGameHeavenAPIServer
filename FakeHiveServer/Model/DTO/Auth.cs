using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO
{
    public class CreateHiveAccountRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class CreateHiveAccountResponse
    {
        [Required]
        public ErrorCode Result { get; set; } = ErrorCode.None;

    }

    public class LoginHiveRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginHiveResponse
    {
        [Required]
        public ErrorCode Result { get; set; } = ErrorCode.None;
        [Required]
        public Int64 PlayerId { get; set; }
        [Required]
        public string Token { get; set; }

    }
}
