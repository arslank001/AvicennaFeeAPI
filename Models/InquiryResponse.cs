namespace AvicennaFeeAPI.Models
{
	public class InquiryResponse
	{
		public string ResponseCode { get; set; }
		public string ConsumerDetail { get; set; }
		public string BillStatus { get; set; }
		public string DueDate { get; set; }
		public string AmountWithinDueDate { get; set; }
		public string AmountAfterDueDate { get; set; }
		public string BillingMonth { get; set; }
		public string DatePaid { get; set; }
		public string AmountPaid { get; set; }
		public string TranAuthId { get; set; }
		public string Reserved { get; set; }

	}
}
