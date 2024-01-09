using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;


namespace APIServer
{
    public class JWT
    {
        const string _secretKey = "kaljf34io8903lajdfdasfsdgr4t4tr32wr32wres3";
        const Int32 _expireMin = 3;

        public static string IssueToken(Int64 player_id)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Name, player_id.ToString()),
                }),
                    Expires = DateTime.UtcNow.AddMinutes(_expireMin),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string userToken = tokenHandler.WriteToken(token);
                return userToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "";
            }
        }

        public static Int64 PlayerIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secretKey);
                var validationParameters = new TokenValidationParameters()
                {
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidIssuer = "",
                    ValidAudience = "",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken validatedToken;
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);
                Int64 player_id = (Int64)((JwtSecurityToken)validatedToken).Payload["player_id"];
                return player_id;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}

public class JWTConfig
{
    public string SecretKey { get; set; }
    public Int32 ExpireMin { get; set; }
}