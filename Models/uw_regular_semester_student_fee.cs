using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace AvicennaFeeAPI.Models
{
	public class uw_regular_semester_student_fee
	{
		[Key]
		public int vno { get; set; } // Unchecked
		public int? sid { get; set; } // Checked
		public double? admission { get; set; } // Checked
		public double? registration { get; set; } // Checked
		public double? security { get; set; } // Checked
		public double? tution { get; set; } // Checked
		public double? allied { get; set; } // Checked
		public double? concession { get; set; } // Checked
		public int? fine { get; set; } // Checked
		public double? arrears { get; set; } // Checked
		public string? instalment { get; set; } // varchar(50) Checked
		public DateTime? duedate { get; set; } // datetime Checked
		public bool? isactive { get; set; } // bit Checked
		public string? remarks { get; set; } // varchar(MAX) Checked
		public string? csession { get; set; } // varchar(10) Checked
		public DateTime? vdate { get; set; } // datetime Checked
		public DateOnly? depositdate { get; set; } // datetime Checked
		public bool? feedeposit { get; set; } // bit Checkedl
		public int? scholorship { get; set; } // Checked
		public bool? barcode { get; set; } // bit Checked
		public string? userid { get; set; } // varchar(50) Checked
		public DateTime? validity_date { get; set; } // datetime Checked
		public int? semester { get; set; } // Checked
		public bool? baf { get; set; } // bit Checked
		public bool? deleted { get; set; } // bit Checked
		public int? printed { get; set; } // Checked
		public string? dtype { get; set; } // varchar(50) Checked
		public string? scholorshiptype { get; set; } // nvarchar(50) Checked
		public double? Differ { get; set; } // Checked
		public int? progid { get; set; } // Checked
		public string? bank_name { get; set; } // nvarchar(50) Checked
		public int? consumer_number { get; set; }
		public string? bank_mnemonic { get; set; }
		public int? tran_auth_id { get; set; }
		public int? transaction_amount { get; set; }
		public DateOnly? tran_date { get; set; }
	    public TimeOnly? tran_time { get; set; }
}
}
