using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace AvicennaFeeAPI.Models
{
	public class uw_regular_semester_student_fee
	{
		[Key]
        public int vno { get; set; }
        public int? sid { get; set; }
        public double? admission { get; set; }
        public double? registration { get; set; }
        public double? security { get; set; }
        public double? tution { get; set; }
        public double? allied { get; set; }
        public double? concession { get; set; }
        public int? fine { get; set; }
        public double? arrears { get; set; }
        public string? instalment { get; set; }
        public DateTime? duedate { get; set; }
        public bool? isactive { get; set; }
        public string? remarks { get; set; }
        public string? csession { get; set; }
        public DateTime? vdate { get; set; }
        public DateTime? depositdate { get; set; }
        public bool? feedeposit { get; set; }
        public int? scholorship { get; set; }
        public bool? barcode { get; set; }
        public string? userid { get; set; }
        public DateTime? validity_date { get; set; }
        public int? semester { get; set; }
        public bool? baf { get; set; }
        public bool? deleted { get; set; }
        public int? printed { get; set; }
        public string? dtype { get; set; }
        public string? scholorshiptype { get; set; }
        public double? differ { get; set; }
        public int? progid { get; set; }
        public string? bank_name { get; set; }
        public string? consumer_number { get; set; }
        public string? bank_mnemonic { get; set; }
        public string? tran_auth_id { get; set; }
        public string? transaction_amount { get; set; }
        public string? tran_date { get; set; }
        public string? tran_time { get; set; }
    }
}
