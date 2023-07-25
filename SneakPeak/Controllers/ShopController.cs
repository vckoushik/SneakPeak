using Microsoft.AspNetCore.Mvc;

namespace SneakPeak.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
