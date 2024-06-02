using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace wepApi.Models
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
