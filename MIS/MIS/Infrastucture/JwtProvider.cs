using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MIS.Models.DB;
using MIS.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MIS.Infrastucture
{
    public interface IJwtProvider
    {
        TokenResponseModel GenerateToken(DbDoctor doctor);
    }
    public class JwtProvider : IJwtProvider
    {
        private readonly JwtOptions _options;

        public JwtProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        // генерация токена
        public TokenResponseModel GenerateToken(DbDoctor doctor)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, doctor.id.ToString()),
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var JwtToken = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(JwtToken);

            return new TokenResponseModel { token = tokenValue };
        }
    }
}
