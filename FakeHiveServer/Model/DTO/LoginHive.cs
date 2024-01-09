using System;
using System.ComponentModel.DataAnnotations;

namespace APIServer.Model.DTO
{
    public class LoginHiveRequest
    {
        [Required]
        [MinLength(1, ErrorMessage = "EMAIL CANNOT BE EMPTY")]
        [StringLength(50, ErrorMessage = "EMAIL IS TOO LONG")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "PASSWORD CANNOT BE EMPTY")]
        [StringLength(30, ErrorMessage = "PASSWORD IS TOO LONG")]
        [DataType(DataType.Password)]
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
