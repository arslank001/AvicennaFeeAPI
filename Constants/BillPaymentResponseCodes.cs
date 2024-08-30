namespace AvicennaFeeAPI.Constants
{
	public class BillPaymentResponseCodes
	{
		public const string Success = "00"; 
		public const string ConsumerNumberNotFound = "01"; 
		public const string UnknownError = "02"; 
		public const string DuplicateTransaction = "03"; 
		public const string InvalidData = "04";
		public const string ServiceFail = "05"; 
		public const string BillAlreadyPaid = "06";
        public const string BillExpired = "07"; 
    }
}
