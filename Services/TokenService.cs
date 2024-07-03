using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AvicennaFeeAPI.Services
{
	public interface ITokenService
	{
		string GenerateToken(string userid);
	}

	public class TokenService : ITokenService
	{
		private readonly string _secret;

		public TokenService(string secret)
		{
			_secret = secret;
		}

		public string GenerateToken(string userid)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_secret);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userid) }),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
