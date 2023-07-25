using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Data;
using SneakPeak.Models;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace SneakPeak.Controllers
{
    [Route("/shop")]    
    public class ProductController : Controller
    {
        SneakPeakDbContext dbContext;
        public ProductController(SneakPeakDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index(string query)
        {
            
            var product = dbContext.Product;
            if (query is null) { 
                return View(product.ToList());
            }
            string cleanedQuery = Regex.Replace(query, @"[^a-zA-Z0-9]", "");
            var results = product.Where(p => p.Name.Contains(cleanedQuery) || p.Brand.Contains(cleanedQuery));
            return View(results.ToList());
        }

        
        [HttpGet] // This attribute specifies that the action responds to HTTP GET requests
        [Route("product/{id:int}")]
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
