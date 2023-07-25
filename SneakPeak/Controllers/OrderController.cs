using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SneakPeak.Repo;
using SneakPeak.Services;

namespace SneakPeak.Controllers
{
    [Authorize]
    [Route("/order")]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IBraintreeService _braintreeService;

        public OrderController(IOrderRepository orderRepo, IBraintreeService braintreeService)
        {
            _orderRepo = orderRepo;
            _braintreeService = braintreeService;
        }



        [Route("UserOrders")]
        public async Task<IActionResult> Index()
        {
   
            var orders = await _orderRepo.UserOrders();
           
            return View(orders);
        }
    }
}
