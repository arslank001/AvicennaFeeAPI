namespace AvicennaFeeAPI.Constants
{
	public class BillPaymentResponseCodes
	{
		public const string Success = "00"; // Successful Bill Payment
		public const string ConsumerNumberNotFound = "01"; // Consumer number does not exist
		public const string UnknownError = "02"; // Unknown Error
		public const string DuplicateTransaction = "03"; // Duplicate Transaction
		public const string InvalidData = "04"; // Invalid Data
		public const string ServiceFail = "05"; // Service Fail
		public const string BillAlreadyPaid = "06"; // Bill Already Paid
	}
}
