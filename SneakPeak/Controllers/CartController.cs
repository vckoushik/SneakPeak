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

        public CartController(SneakPeakDbContext dbContext, ICartRepository cartRepo, IBraintreeService braintreeService, IAddressRepository addressRepository)
        {
           this.dbContext= dbContext;
            _cartRepo  = cartRepo;
            _braintreeService = braintreeService;
            _addressRepository = addressRepository; 
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
                return Ok(cartCount);
            }
            return RedirectToAction("GetUserCart");
        }

        [Route("RemoveItem/{productId:int}")]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cartRepo.RemoveItem(productId);
          
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
                return RedirectToAction("ThankYou", "Home");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
