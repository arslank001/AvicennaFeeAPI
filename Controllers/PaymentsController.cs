using AvicennaFeeAPI.Constants;
using AvicennaFeeAPI.Data;
using AvicennaFeeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;

namespace AvicennaFeeAPI.Controllers
{

	[ApiController]
	[Route("api/Payments")]
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

		    	var inquiryResult = await _context.uw_regular_semester_student_fee.Where(b => b.consumer_number == request.ConsumerNumber).FirstOrDefaultAsync();
				//var inquiryResult = (from UwRegularFee in _context.uw_regular_semester_student_fee
				//join StudentInfo in _context.uw_student on UwRegularFee.sid equals StudentInfo.stuid
				//where UwRegularFee.consumer_number == request.ConsumerNumber
				//select new { uw_regular_semester_student_fee = UwRegularFee, uw_student = StudentInfo }).SingleOrDefault();

				if (inquiryResult == null)
				{
					return NotFound(new { ResponseCode = BillIquiryResponseCodes.ConsumerNumberNotFound, Message = "Consumer number does not exist" });
				}

				//Fine
				int Fine = 0;
				if (inquiryResult.duedate < DateTime.Now.Date)
				{
					Fine = Convert.ToInt32(inquiryResult.fine);
				}

			    //Bill Status
			    string BillStatus = "";
		     	string ResponseCode = "";
		    	string Message = "";
				if (inquiryResult.feedeposit == true)
				{
					BillStatus = "P";
					ResponseCode = BillIquiryResponseCodes.BillAlreadyPaid;
					Message = "Bill already paid!";
				}
				else if (string.IsNullOrEmpty(inquiryResult.feedeposit.ToString()) && inquiryResult.validity_date >= DateTime.Now.Date && inquiryResult.deleted != true)
				{
					BillStatus = "U";
					ResponseCode = BillIquiryResponseCodes.Success;
					Message = "Successfull bill inquiry!";
				}
				else if (string.IsNullOrEmpty(inquiryResult.feedeposit.ToString()) && inquiryResult.validity_date < DateTime.Now.Date && inquiryResult.deleted != true)
				{
					BillStatus = "E";
					ResponseCode = BillIquiryResponseCodes.BillExpired;
					Message = "Bill Expired!";
				}
				else if (inquiryResult.deleted == true)
				{
					BillStatus = "B";
					ResponseCode = BillIquiryResponseCodes.ConsumerNumberBlock;
					Message = "Consumer Number Block!";
				}

				//Total fee calculation
				int totalAmount = Convert.ToInt32(inquiryResult.tution) + Convert.ToInt32(inquiryResult.allied) + Convert.ToInt32(inquiryResult.arrears) - Convert.ToInt32(inquiryResult.concession);

				//Generating a response
				var response = new InquiryResponse
				{
					ResponseCode = ResponseCode,
					ConsumerDetail = inquiryResult.sid?.ToString() ?? string.Empty,
					BillStatus = BillStatus,
					DueDate = inquiryResult.duedate?.ToString("yyyyMMdd") ?? string.Empty,
                    ValidityDate = inquiryResult.validity_date?.ToString("yyyyMMdd") ?? string.Empty,
                    AmountWithinDueDate = totalAmount.ToString() ?? string.Empty,
					AmountAfterDueDate = (totalAmount + Fine).ToString(),
			        BillingMonth = inquiryResult.duedate?.ToString("yyMM") ?? string.Empty,
					DatePaid = inquiryResult.tran_date?.ToString("yyyyMMdd") ?? string.Empty,
					AmountPaid = inquiryResult.transaction_amount?.ToString() ?? string.Empty,
					TranAuthId = inquiryResult.tran_auth_id?.ToString() ?? string.Empty,
					//Reserved = request.Reserved.
				};

				return Ok ( new { ResponseCode = ResponseCode, Message = Message, Data = response });

		     }
			catch (Exception ex)
			{
				return StatusCode(500, new { ResponseCode = ResponseCodes.UnknownError, Message = "An unknown error occurred", Details = ex.Message });
			}
		}

		[HttpPost("BillPayment")]
		public async Task<IActionResult> BillPayment([FromBody] BillPaymentRequest request)
		{
			try
			{
				// Validate request
				if (request == null || string.IsNullOrEmpty(request.ConsumerNumber.ToString()) || string.IsNullOrEmpty(request.BankMnemonic))
				{
					return BadRequest(new { ResponseCode = ResponseCodes.InvalidData, Message = "Invalid data" });
				}

				//var bill = await _context.uw_regular_semester_student_fee
				//	.Where(b => b.ConsumerNumber == request.ConsumerNumber && b.BankMnemonic == request.BankMnemonic)
				//	.FirstOrDefaultAsync();
                var PaymentResult = await _context.uw_regular_semester_student_fee.Where(b => b.consumer_number == request.ConsumerNumber && b.bank_mnemonic == request.BankMnemonic).FirstOrDefaultAsync();

                if (PaymentResult == null)
				{
					return NotFound(new { ResponseCode = ResponseCodes.ConsumerNumberNotFound, Message = "Consumer number does not exist" });
				}

				if (PaymentResult.feedeposit.ToString() == "True")
				{
					return BadRequest(new { ResponseCode = ResponseCodes.BillAlreadyPaid, Message = "Bill has already been paid" });
				}

                // Process the payment
                PaymentResult.tran_auth_id = request.TranAuthId;
                PaymentResult.feedeposit = true;
				PaymentResult.depositdate = request.TranDate;
                PaymentResult.tran_time = request.TranTime;		
				PaymentResult.bank_mnemonic = request.BankMnemonic;
                PaymentResult.transaction_amount = request.TransactionAmount;

				_context.uw_regular_semester_student_fee.Update(PaymentResult);
				await _context.SaveChangesAsync();

				return Ok(new
				{
					ResponseCode = ResponseCodes.Success,
					Message = "Bill payment successful",
					Data = PaymentResult
                });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { ResponseCode = ResponseCodes.UnknownError, Message = "An unknown error occurred", Details = ex.Message });
			}
		}
	}
}
