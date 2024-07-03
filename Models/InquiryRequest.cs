using System.Numerics;

namespace AvicennaFeeAPI.Models
{
	public class InquiryRequest
	{
		public int? ConsumerNumber { get; set; }
		public string? BankMnemonic { get; set; }
		public string? Reserved { get; set; }
	}
}
