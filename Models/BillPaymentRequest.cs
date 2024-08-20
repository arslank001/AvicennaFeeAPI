using System.Numerics;

namespace AvicennaFeeAPI.Models
{
	public class BillPaymentRequest
	{
		public string? ConsumerNumber { get; set; }
		public string? TranAuthId { get; set; } 
		public string? TransactionAmount { get; set; }
		public string? TranDate { get; set; }
		public string? TranTime { get; set; }
		public string? BankMnemonic { get; set; }
		public string? Reserved { get; set; }
	}
}
