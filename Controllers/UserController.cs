using AvicennaFeeAPI.Data;
using AvicennaFeeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvicennaFeeAPI.Controllers
{
	[Authorize]
	[ApiController]
	[Route("[controller]")]
	public class UserController : Controller
	{
		private readonly ApplicationContext _context;

		public UserController(ApplicationContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<uw_users_login>>> GetUsers()
		{
			return await _context.uw_users_login.ToListAsync();
		}

		[HttpPost]
		public async Task<ActionResult<uw_users_login>> PostUser(uw_users_login user)
		{
			_context.uw_users_login.Add(user);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(GetUsers), new { id = user.uid }, user);
		}
	}
}
