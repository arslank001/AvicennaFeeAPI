namespace AvicennaFeeAPI.Constants
{
	public class BillIquiryResponseCodes
	{
		public const string Success = "00"; // Successful Bill Payment
		public const string ConsumerNumberNotFound = "01"; // Consumer number does not exist
		public const string ConsumerNumberBlock = "02"; // Consumer Number Block
		public const string UnknownError = "03"; // Unknown Error
		public const string InvalidData = "04"; // Invalid Data
		public const string ServiceFail = "05"; // Service Fail
		public const string BillAlreadyPaid = "06"; // Bill Already Paid
		public const string BillExpired = "07"; // Bill is Expired
	}
}
