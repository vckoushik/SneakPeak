using Microsoft.AspNetCore.Mvc;

namespace SneakPeak.Controllers
{
    [Route("/checkout")]
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


    }
}
