using System.ComponentModel.DataAnnotations;

namespace AvicennaFeeAPI.Models
{
	public class uw_users_login
	{
		[Key]
		public int uid { get; set; }
		public string? userid { get; set; }
		public string? password { get; set; }
		public string? tpassword { get; set; }
		public string? email { get; set; }
		public bool? rememberme { get; set; }
		public bool? isactive { get; set; }
		public string? remarks { get; set; }
		public int? gid { get; set; }
		public int? progid { get; set; }
		public int? clearnacesectionid { get; set; }
		public int? fid { get; set; }
		public bool? ispush { get; set; }
		public string? device_id { get; set; }
		public string? ppassword { get; set; }
		public string? sonid { get; set; }
	}
}
