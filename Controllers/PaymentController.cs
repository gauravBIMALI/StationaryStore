using ClzProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserRoles.Data;

namespace ClzProject.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        }

        [HttpGet]
        public async Task<IActionResult> InitiateEsewaPayment(int orderId)
        {
            try
            {
                // Log the incoming orderId
                Console.WriteLine($"=== InitiateEsewaPayment called with orderId: {orderId} ===");

                var userId = GetCurrentUserId();
                Console.WriteLine($"Current userId: {userId}");

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("ERROR: User not authenticated");
                    TempData["ErrorMessage"] = "User not authenticated";
                    return RedirectToAction("Login", "Account");
                }

                Console.WriteLine("Fetching order from database...");
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.BuyerId == userId);

                Console.WriteLine($"Order found: {order != null}");

                if (order == null)
                {
                    Console.WriteLine($"ERROR: Order not found. OrderId: {orderId}, UserId: {userId}");
                    TempData["ErrorMessage"] = "Order not found or you don't have permission to access it";
                    return RedirectToAction("MyOrders", "Home");
                }

                Console.WriteLine($"Order Number: {order.OrderNumber}");
                Console.WriteLine($"Payment Status: {order.PaymentStatus}");
                Console.WriteLine($"Payment Method: {order.PaymentMethod}");
                Console.WriteLine($"OrderItems Count: {order.OrderItems?.Count ?? 0}");

                if (order.PaymentStatus == "Paid")
                {
                    Console.WriteLine("Order already paid - redirecting");
                    TempData["SuccessMessage"] = "This order is already paid";
                    return RedirectToAction("OrderDetails", "Home", new { id = orderId });
                }

                if (order.PaymentMethod != "Esewa")
                {
                    Console.WriteLine($"ERROR: Invalid payment method: {order.PaymentMethod}");
                    TempData["ErrorMessage"] = "This order is not set for eSewa payment";
                    return RedirectToAction("OrderDetails", "Home", new { id = orderId });
                }

                if (order.OrderItems == null || !order.OrderItems.Any())
                {
                    Console.WriteLine("ERROR: No order items found");
                    TempData["ErrorMessage"] = "Order items not found";
                    return RedirectToAction("MyOrders", "Home");
                }

                Console.WriteLine("=== All validations passed, returning view ===");
                return View(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== EXCEPTION in InitiateEsewaPayment ===");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                Console.WriteLine($"InnerException: {ex.InnerException?.Message}");

                TempData["ErrorMessage"] = $"Failed to load payment page: {ex.Message}";
                return RedirectToAction("MyOrders", "Home");
            }
        }


        // eSewa Success Callback
        [HttpGet]
        public async Task<IActionResult> EsewaSuccess(string oid, string amt, string refId, string pid)
        {
            try
            {
                // prefer pid if present (eSewa usually sends pid)
                var transactionId = !string.IsNullOrEmpty(pid) ? pid : oid;

                // Find payment record by transaction ID (check both just in case)
                var payment = await _context.EsewaPayments
                    .Include(p => p.Order)
                    .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

                if (payment == null)
                {
                    TempData["ErrorMessage"] = "Payment record not found";
                    return RedirectToAction("MyOrders", "Home");
                }

                // Verify payment with eSewa (use the amount returned by eSewa if necessary)
                var amountToVerify = amt ?? payment.TotalAmount.ToString("0.00");
                var isVerified = await VerifyEsewaPayment(amountToVerify, refId, transactionId);

                if (isVerified)
                {
                    payment.Status = "Success";
                    payment.EsewaTransactionCode = refId;
                    payment.RefId = refId;
                    payment.CompletedAt = DateTime.UtcNow;

                    if (payment.Order != null)
                    {
                        payment.Order.PaymentStatus = "Paid";
                        payment.Order.OrderStatus = "Confirmed";
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Payment successful! Your order has been confirmed.";
                    return RedirectToAction("PaymentSuccess", new { transactionId = transactionId });
                }
                else
                {
                    payment.Status = "Failed";
                    await _context.SaveChangesAsync();

                    TempData["ErrorMessage"] = "Payment verification failed";
                    return RedirectToAction("PaymentFailed", new { transactionId = transactionId });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EsewaSuccess Exception: " + ex);
                TempData["ErrorMessage"] = "Payment processing failed";
                return RedirectToAction("MyOrders", "Home");
            }
        }


        // eSewa Failure Callback
        [HttpGet]
        public async Task<IActionResult> EsewaFailure(string pid)
        {
            try
            {
                var payment = await _context.EsewaPayments
                    .FirstOrDefaultAsync(p => p.TransactionId == pid);

                if (payment != null)
                {
                    payment.Status = "Failed";
                    await _context.SaveChangesAsync();
                }

                TempData["ErrorMessage"] = "Payment was cancelled or failed";
                return RedirectToAction("PaymentFailed", new { transactionId = pid });
            }
            catch
            {
                return RedirectToAction("MyOrders", "Home");
            }
        }

        private async Task<bool> VerifyEsewaPayment(string amount, string refId, string transactionId)
        {
            try
            {
                var esewaConfig = _configuration.GetSection("Esewa");
                var verificationUrl = esewaConfig["VerificationUrl"];
                var merchantCode = esewaConfig["MerchantCode"];

                using (var httpClient = new HttpClient())
                {
                    var values = new List<KeyValuePair<string, string>>
            {
                new("amt", amount),
                new("rid", refId),
                new("pid", transactionId),
                new("scd", merchantCode)
            };

                    var content = new FormUrlEncodedContent(values);
                    var response = await httpClient.PostAsync(verificationUrl, content);
                    var xmlResponse = await response.Content.ReadAsStringAsync();

                    // eSewa sandbox sends: <response><response_code>Success</response_code></response>
                    return xmlResponse.Contains("<response_code>Success</response_code>");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"eSewa verification failed: {ex.Message}");
                return false;
            }
        }




        // Payment Success Page
        [HttpGet]
        public async Task<IActionResult> PaymentSuccess(string transactionId)
        {
            var payment = await _context.EsewaPayments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            if (payment == null)
            {
                return RedirectToAction("MyOrders", "Home");
            }

            return View(payment);
        }

        // Payment Failed Page
        [HttpGet]
        public async Task<IActionResult> PaymentFailed(string transactionId)
        {
            var payment = await _context.EsewaPayments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId);

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessEsewaPayment(int orderId)
        {
            try
            {
                var userId = GetCurrentUserId();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.OrderId == orderId && o.BuyerId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Order not found";
                    return RedirectToAction("MyOrders", "Home");
                }

                if (order.PaymentStatus == "Paid")
                {
                    TempData["ErrorMessage"] = "This order is already paid";
                    return RedirectToAction("OrderDetails", "Home", new { id = orderId });
                }

                // Generate unique transaction ID
                var transactionId = $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{orderId}";

                // Calculate amounts
                decimal amount = order.TotalAmount;
                decimal taxAmount = 0;
                decimal serviceCharge = 0;
                decimal deliveryCharge = order.DeliveryFee;
                decimal totalAmount = amount + taxAmount + serviceCharge + deliveryCharge;

                // Create payment record
                var payment = new EsewaPayment
                {
                    OrderId = orderId,
                    TransactionId = transactionId,
                    Amount = amount,
                    TaxAmount = taxAmount,
                    ServiceCharge = serviceCharge,
                    DeliveryCharge = deliveryCharge,
                    TotalAmount = totalAmount,
                    ProductId = order.OrderNumber,
                    BuyerId = userId,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.EsewaPayments.Add(payment);
                await _context.SaveChangesAsync();

                // Prepare eSewa payment data
                var esewaConfig = _configuration.GetSection("Esewa");

                ViewBag.Amount = amount.ToString("0.00");
                ViewBag.TaxAmount = taxAmount.ToString("0.00");
                ViewBag.ServiceCharge = serviceCharge.ToString("0.00");
                ViewBag.DeliveryCharge = deliveryCharge.ToString("0.00");
                ViewBag.TotalAmount = totalAmount.ToString("0.00");
                ViewBag.TransactionUuid = transactionId;
                ViewBag.ProductCode = esewaConfig["MerchantCode"];
                ViewBag.ProductServiceCharge = "0";
                ViewBag.ProductDeliveryCharge = deliveryCharge.ToString("0.00");
                ViewBag.SuccessUrl = esewaConfig["SuccessUrl"];
                ViewBag.FailureUrl = esewaConfig["FailureUrl"];
                ViewBag.PaymentUrl = esewaConfig["PaymentUrl"];

                // ✅ This returns the EsewaPayment view
                return View("EsewaPayment");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessEsewaPayment: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to initiate payment";
                return RedirectToAction("MyOrders", "Home");
            }
        }

        // Test action to verify routing
        [HttpGet]
        public IActionResult Test()
        {
            return Content("Payment Controller is working!");
        }
    }
}