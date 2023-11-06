
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Models;
using SneakPeak.Repo;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.RegularExpressions;

namespace SneakPeak.Controllers
{
    [Authorize]
    [Route("/")]
    public class HomeController : Controller
    {
        
        private readonly UserManager<SneakPeakUser> _userManager;
        private readonly IAddressRepository _addressRepository;

        public HomeController(UserManager<SneakPeakUser> UserManager,IAddressRepository addressRepository)
        {
            _userManager = UserManager; 
            _addressRepository = addressRepository;
            
        }
        [HttpGet("")] 
        public IActionResult Index()
        {
            ViewData["UserID"]= _userManager.GetUserId(this.User);
            return View();
        }
        [HttpGet("Privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost("SaveContact")]
        public async Task<IActionResult> SaveContact(Contact contact)
        {
            try
            {
                await _addressRepository.SaveContact(contact);
            }
            catch(Exception ex)
            {
                return RedirectToAction("Contact", "Home");
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpGet("Address")]
        public async Task<IActionResult> Address()
        {
            Address address = await _addressRepository.UserAddress();
            return View(address);
        }
        [HttpPost("SaveAddress")]
        public async Task<IActionResult> SaveAddress(Address address)
        {
            try
            {
                Address addressEx = await _addressRepository.UserAddress();

                if (addressEx != null) { address.Id = addressEx.Id; }
                else { address.Id = 0; }
                bool res = IsPhoneValid(address.Phone);
                if (res)
                {
                    await _addressRepository.SaveAddress(address);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
             
                    return RedirectToAction("Address", "Home");
                }

                
            }
            catch(Exception)
            {
                return RedirectToAction("Address", "Home");
            }

            // If the model is not valid, redisplay the form with validation errors
           // return View("Address",address);
        }

        [HttpGet("ThankYou")]
        public IActionResult ThankYou()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public static bool IsPhoneValid(string phoneNumber)
        {
            string regex = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
            if (phoneNumber != null)
                return Regex.IsMatch(phoneNumber, regex);
            else return false;
        }
    }
}