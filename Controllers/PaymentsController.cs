using AvicennaFeeAPI.Constants;
using AvicennaFeeAPI.Data;
using AvicennaFeeAPI.Models;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace AvicennaFeeAPI.Controllers
{

	[ApiController]
	[Route("Api/Payments")]
	public class PaymentsController : Controller
	{
		private readonly ApplicationContext _context;
		private readonly IHealthCheckService _healthCheckService;
		public PaymentsController(ApplicationContext context, IHealthCheckService healthCheckService)
		{
			_context = context;
			_healthCheckService = healthCheckService;
		}

		[Authorize]
		[HttpPost("BillInquiry")]
		public async Task<IActionResult> BillInquiry([FromBody] InquiryRequest request)
		{
			try
			{
                string BillStatus = "";

                // Validate request
                if (request == null || string.IsNullOrEmpty(request.ConsumerNumber.ToString()) || string.IsNullOrEmpty(request.BankMnemonic))
				{
					return BadRequest(new { ResponseCode = BillIquiryResponseCodes.InvalidData, Message = "Invalid data" });
				}

				// Check if the service is healthy
				bool serviceFail = !await _healthCheckService.IsServiceHealthy();
				if (serviceFail)
				{
					return StatusCode(StatusCodes.Status500InternalServerError, new { ResponseCode = BillIquiryResponseCodes.ServiceFail, Message = "Service fail." });
				}
				else
				{
					var inquiryResult = await _context.uw_regular_semester_student_fee.Where(b => b.consumer_number == request.ConsumerNumber).FirstOrDefaultAsync();

					if (inquiryResult == null)
					{
						return NotFound(new { ResponseCode = BillIquiryResponseCodes.ConsumerNumberNotFound, Message = "Consumer number does not exist" });
					}
					else if (inquiryResult.feedeposit == true && !string.IsNullOrEmpty(inquiryResult.depositdate.ToString()))
					{
						return BadRequest(new { Message = "Bill already paid", ResponseCode = BillIquiryResponseCodes.BillAlreadyPaid, BillStatus = "P" });
					}
					else if (string.IsNullOrEmpty(inquiryResult.feedeposit.ToString()) && inquiryResult.validity_date < DateTime.Now.Date && inquiryResult.deleted != true)
					{
						return BadRequest(new { Message = "Bill expired", ResponseCode = BillIquiryResponseCodes.BillExpired, BillStatus = "E" });
					}
					else if (inquiryResult.deleted == true)
					{
						return BadRequest(new { Message = "Consumer number block", ResponseCode = BillIquiryResponseCodes.ConsumerNumberBlock, BillStatus = "B" });
					}
					else if (string.IsNullOrEmpty(inquiryResult.feedeposit.ToString()) && inquiryResult.validity_date >= DateTime.Now.Date && inquiryResult.deleted != true)
					{
						// Retrieve the student record using the sid from the payment record
						var StudentData = await _context.uw_student.Where(s => s.stuid == inquiryResult.sid).FirstOrDefaultAsync();

						//Total fee calculation
						int TotalAmount = Convert.ToInt32(inquiryResult.tution) + Convert.ToInt32(inquiryResult.allied) + Convert.ToInt32(inquiryResult.arrears) - Convert.ToInt32(inquiryResult.concession);

						//Generating a response
						var ResponseBillInquiry = new InquiryResponse
						{
                            ResponseCode = BillIquiryResponseCodes.Success,                  
                            ConsumerDetail = StudentData.sname.ToString() ?? string.Empty,
                            BillStatus = "U",
                            DueDate = inquiryResult.duedate?.ToString("yyyyMMdd") ?? string.Empty,
							ValidityDate = inquiryResult.validity_date?.ToString("yyyyMMdd") ?? string.Empty,
							AmountWithinDueDate = FormatAmount(TotalAmount).ToString() ?? string.Empty,
							AmountAfterDueDate = FormatAmount(TotalAmount + (inquiryResult.fine ?? 0)).ToString(),
							BillingMonth = inquiryResult.duedate?.ToString("yyMM") ?? string.Empty,
							DatePaid = "",
							AmountPaid = "",
							TranAuthId = "",
							Reserved = ""
						};
						return Ok(new { Message = "Successful bill inquiry", Data = ResponseBillInquiry });
					}
				}
                return BadRequest(new { ResponseCode = BillIquiryResponseCodes.UnknownError, Message = "Unknown error" });
		    }
            catch (Exception ex)
            {
            	return StatusCode(500, new { ResponseCode = ResponseCodes.UnknownError, Message = "Unknown error", Details = ex.Message });
            }
        }

		[Authorize]
		[HttpPost("BillPayment")]
		public async Task<IActionResult> BillPayment([FromBody] BillPaymentRequest request)
		{
			try
			{
				// Validate request
				if (request == null || string.IsNullOrEmpty(request.ConsumerNumber.ToString()) || string.IsNullOrEmpty(request.BankMnemonic))
                {
                    return BadRequest(new { ResponseCode = BillPaymentResponseCodes.InvalidData, Message = "Invalid data" });
                }

                // Check if the service is healthy
                bool ServiceFail = !await _healthCheckService.IsServiceHealthy();
                if (ServiceFail)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { ResponseCode = BillPaymentResponseCodes.ServiceFail, Message = "Service fail" });
                }
                else
                {
                    var PaymentResult = await _context.uw_regular_semester_student_fee.Where(b => b.consumer_number == request.ConsumerNumber).FirstOrDefaultAsync();

                    if (PaymentResult == null)
                    {
                        return NotFound(new { ResponseCode = BillPaymentResponseCodes.ConsumerNumberNotFound, Message = "Consumer number does not exist" });
                    }
                    else if (PaymentResult.consumer_number == request.ConsumerNumber && PaymentResult.tran_auth_id == request.TranAuthId && PaymentResult.tran_date == request.TranDate && PaymentResult.tran_time == request.TranTime)
                    {
                        return BadRequest(new { ResponseCode = BillPaymentResponseCodes.DuplicateTransaction, Message = "Duplicate Transaction" });
                    }
                    else if (PaymentResult.feedeposit == true && PaymentResult.tran_auth_id != null)
                    {
                        return BadRequest(new { ResponseCode = BillPaymentResponseCodes.BillAlreadyPaid, Message = "Bill already paid" });
                    }
                    else if (string.IsNullOrEmpty(PaymentResult.feedeposit.ToString()) && PaymentResult.validity_date >= DateTime.Now.Date && PaymentResult.deleted != true && PaymentResult.tran_auth_id == null)
                    {
                        if (request.ConsumerNumber != null && request.TranAuthId != null && request.TranDate != null && request.TranTime != null && PaymentResult.tran_auth_id == null)
                        {
                            // Retrieve the student record using the sid from the payment record
                             var StudentData = await _context.uw_student.Where(s => s.stuid == PaymentResult.sid).FirstOrDefaultAsync();

                            // Process the payment
                            PaymentResult.tran_auth_id = request.TranAuthId;
                            PaymentResult.feedeposit = true;
                            PaymentResult.depositdate = DateTime.Now;
                            PaymentResult.tran_date = request.TranDate;
                            PaymentResult.tran_time = request.TranTime;
                            PaymentResult.bank_mnemonic = request.BankMnemonic;
                            PaymentResult.transaction_amount = RemoveExtraZeros(request.TransactionAmount);

                            _context.uw_regular_semester_student_fee.Update(PaymentResult);
                            await _context.SaveChangesAsync();

                            var ResponseBillPayment = new BillPaymentResponse
                            {
                                ResponseCode = BillPaymentResponseCodes.Success,
                                IdentificationParameter = StudentData.sname,
                                Reserved = ""
                            };

                            return Ok(new { Message = "Successful bill payment", Data = ResponseBillPayment });
                        }
                    }
                    else if (string.IsNullOrEmpty(PaymentResult.feedeposit.ToString()) && PaymentResult.validity_date < DateTime.Now.Date && PaymentResult.deleted != true && PaymentResult.tran_auth_id == null)
                    {
                        return BadRequest(new { ResponseCode = BillPaymentResponseCodes.BillExpired, Message = "Bill expired" });
                    }
                }
                return BadRequest(new { ResponseCode = BillPaymentResponseCodes.UnknownError, Message = "Unknown error" });
		    }
            catch (Exception ex)
            {
                return StatusCode(500, new { ResponseCode = BillPaymentResponseCodes.UnknownError, Message = "Unknown error", Details = ex.Message });
            }
        }


        string FormatAmount(decimal amount)
        {
            // Multiply by 100 to shift decimal places (e.g., 120.00 becomes 12000)
            long AmountInMinorUnits = (long)(amount * 100);

            // Determine the sign (positive or negative)
            string Sign = AmountInMinorUnits >= 0 ? "+" : "-";

            // Convert the absolute value of the amount to a string
            string AmountString = Math.Abs(AmountInMinorUnits).ToString();

            // Pad with zeros on the left to make it 13 digits long
            string PaddedAmount = AmountString.PadLeft(13, '0');

            // Combine the sign with the padded amount
            return Sign + PaddedAmount;
        }

        string RemoveExtraZeros(string TranAmount)
        {
            // Remove the two trailing zeros from the right side
            string withoutTrailingZeros = TranAmount.Substring(0, TranAmount.Length - 2);

            // Trim leading zeros from the left side
            string trimmedAmount = withoutTrailingZeros.TrimStart('0');

            return trimmedAmount;
        }
    }
}