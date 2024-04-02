using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Data;
using SneakPeak.Models;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using SneakPeak.Services;

namespace SneakPeak.Controllers
{
    [Route("/shop")]    
    public class ProductController : Controller
    {
        private readonly SneakPeakDbContext dbContext;
        private readonly IMailService _mailService;
        public ProductController(SneakPeakDbContext dbContext,IMailService mailService)
        {
            this.dbContext = dbContext;
            _mailService = mailService;
        }

        public IActionResult Index(string query, string category)
        {
            
            var product = dbContext.Product;

            if (query is null) { 
                if(category is null)
                 return View(product.ToList());
                else
                 return View(product.Where(p => p.Category.Equals(category)));
            }
          
            string cleanedQuery = Regex.Replace(query, @"[^a-zA-Z0-9]", "");
            var results = product.Where(p => p.Name.Contains(cleanedQuery) || p.Brand.Contains(cleanedQuery));
            return View(results.ToList());
        }

        
        [HttpGet] // This attribute specifies that the action responds to HTTP GET requests
        [Route("details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await dbContext.Product.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

    }
}
