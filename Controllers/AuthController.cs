using AvicennaFeeAPI.Data;
using AvicennaFeeAPI.Models;
using AvicennaFeeAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AvicennaFeeAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthController : Controller
	{
		private readonly ApplicationContext _context;
		private readonly ITokenService _tokenService;

		public AuthController(ApplicationContext context, ITokenService tokenService)
		{
			_context = context;
			_tokenService = tokenService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] uw_users_login user)
		{
			var dbUser = await _context.uw_users_login.SingleOrDefaultAsync(u => u.userid == user.userid && u.password == user.password);
			if (dbUser == null)
				return Unauthorized();

			var token = _tokenService.GenerateToken(user.userid);
			return Ok(new { token });
		}
	}
}
