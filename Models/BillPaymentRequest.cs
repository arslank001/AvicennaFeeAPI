namespace AvicennaFeeAPI.Models
{
	public class BillPaymentRequest
	{
		public int ConsumerNumber { get; set; }
		public int TranAuthId { get; set; } 
		public int TransactionAmount { get; set; }
		public DateOnly TranDate { get; set; }
		public TimeOnly TranTime { get; set; }
		public string BankMnemonic { get; set; }
		public string Reserved { get; set; }
	}
}
