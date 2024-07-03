using AvicennaFeeAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AvicennaFeeAPI.Data
{
	public class ApplicationContext : DbContext
	{
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

		public DbSet<uw_users_login> uw_users_login { get; set; }
		public DbSet<uw_regular_semester_student_fee> uw_regular_semester_student_fee { get; set; }

		public DbSet<uw_student> uw_student { get; set; }
	}
}
