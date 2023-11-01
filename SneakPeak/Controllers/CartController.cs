using Azure;
using Braintree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SneakPeak.Data;
using SneakPeak.Models;
using SneakPeak.Repo;
using SneakPeak.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace SneakPeak.Controllers
{
    [Authorize]
    [Route("/Cart")]
    public class CartController : Controller
    {
        private readonly SneakPeakDbContext dbContext;
        private readonly ICartRepository _cartRepo;
        private readonly IBraintreeService _braintreeService;
        private readonly IAddressRepository _addressRepository;
        private readonly IMailService _mailService;

        public CartController(SneakPeakDbContext dbContext, ICartRepository cartRepo, IBraintreeService braintreeService, IAddressRepository addressRepository,IMailService mailService)
        {
           this.dbContext= dbContext;
            _cartRepo  = cartRepo;
            _braintreeService = braintreeService;
            _addressRepository = addressRepository; 
            _mailService = mailService; 
            
        }


       

        // GET: Cart
        public IActionResult Index()
        {
            return View();
        }

       
        [Route("AddItem/{productId:int}")]
        public async Task<IActionResult> AddItem(int productId,int qty=1,int redirect=0)
        {
            var cartCount= await _cartRepo.AddItem(productId,qty);
            if (redirect == 0) {
                TempData["success"] = "Your Product Added to Cart";
                return Ok(cartCount);
            }
            return RedirectToAction("GetUserCart");
        }

        [Route("RemoveItem/{productId:int}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cartRepo.RemoveItem(productId);
            TempData["success"] = "Product Removed from Cart";
            return RedirectToAction("GetUserCart");

        }

        [Route("GetUserCart")]
        public  async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepo.GetUserCart();
            return View(cart);
        }

        [Route("GetTotalItemInCart")]
        public async Task<IActionResult> GetTotalItemInCart()
        {
            int cartItem=await _cartRepo.GetCartItemsCount();
            return Ok(cartItem);
        }

        [Route("Checkout")]
        public async Task<IActionResult> MakePayment()
        {
            var address =await _addressRepository.UserAddress();
            var gateway = _braintreeService.GetGateway();
            var clientToken = gateway.ClientToken.Generate();  //Genarate a token
            ViewBag.ClientToken = clientToken;


            var cart = await _cartRepo.GetUserCart();
            


            var data = new OrderPurchaseVM
            {
                Id = cart.Id,
                Items = cart.Items, 
                UserId= cart.UserId,
                Address=address,
                Nonce = ""
            };

            return View(data);

        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(OrderPurchaseVM model)
        {
            var gateway = _braintreeService.GetGateway();

            var cart = await _cartRepo.GetUserCart();
            List<CartItem> cartItems= new List<CartItem>(); 
            cartItems.AddRange(cart.Items);

            decimal total = 0;

            foreach (var cartItem in cart.Items)
            {
                total += cartItem.PricePerUnit * cartItem.Quantity;
            }
            var request = new TransactionRequest
            {
                Amount = total,
                PaymentMethodNonce = model.Nonce,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            Result<Transaction> result = gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                bool isCheckedOut = await _cartRepo.DoCheck();
                if (!isCheckedOut)
                {
                    throw new Exception("Something went wrong");
                }
                //MailData mailData = new MailData();
                //mailData.EmailSubject = "Order Success";
                //mailData.EmailToName = "koushiksiva9@gmail.com";
                //mailData.EmailToId = "koushiksiva9@gmail.com";
                //mailData.EmailBody = "Payment Successful and Order Placed";
                string emailTemplate = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <title>Order Confirmation</title>
</head>
<body>
    <table width='100%' border='0' cellspacing='0' cellpadding='0'>
        <tr>
            <td align='center'>
                <table width='600' border='0' cellspacing='0' cellpadding='0' style='border: 1px solid #ddd; border-collapse: collapse;'>
                    <tr>
                        <td align='center' style='background-color: #f5f5f5; padding: 20px;'>
                            <h1>Your Order Confirmation</h1>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding: 20px;'>
                            <p>Dear Customer,</p>
                            <p>Thank you for placing an order with us. Your order details are as follows:</p>
                            <table width='100%' border='1' cellspacing='0' cellpadding='10'>
                                <tr>
                                    <th>Product</th>
                                    <th>Quantity</th>
                                    <th>Price</th>
                                </tr>";
                emailTemplate = emailTemplate + GenerateProductRows(cartItems);

                emailTemplate = emailTemplate + $@"
                            </table>
                            <p>Order Total: "+ total+ $@"</p>
                            <p>Your order will be shipped to:</p>
                            <p>If you have any questions, please contact our customer support at sneakpeak@gmail.com or 9409409400.</p>
                            <p>Thank you for choosing us!</p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";

                // You can use the 'emailTemplate' string in your code to generate emails.

                Message message = new Message(new string[] { "koushiksiva9@gmail.com" }, "SneakPeak - Order Places Successfully", emailTemplate);
                bool value = _mailService.SendEmail(message);
                return RedirectToAction("ThankYou", "Home");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }

        string GenerateProductRows(IEnumerable<CartItem> items)
        {
            string productRows = $@"";

            foreach (var item in items)
            {
                productRows= productRows + $@"<tr><td>"+item.Product.Name+ $@"</td><td>"+item.Quantity+ $@"</td><td>"+item.Product.Price + $@"</td></tr>";
            }

            return productRows;
        }
    }
}
