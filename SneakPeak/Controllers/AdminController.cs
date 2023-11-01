using MailKit.Search;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using SneakPeak.Data;
using SneakPeak.Models;
using SneakPeak.Repo;

namespace SneakPeak.Controllers
{
    public class AdminController : Controller
    {
        private readonly SneakPeakDbContext _db;
        private readonly  IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        public AdminController(SneakPeakDbContext db,IProductRepository productRepository, IOrderRepository orderRepositor)
        {
            _productRepository = productRepository;
            _orderRepository= orderRepositor;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Route("/admin/productindex")]
        public async Task<IActionResult> ProductIndex()
        {
            var products = await _productRepository.GetProducts();

            return View(products);
        }
       
        [Route("/admin/ProductCreate")]
		public async Task<IActionResult> ProductCreate()
		{
            return View();
		}
        

        [HttpPost]
        [Route("/admin/ProductCreate")]
        public async Task<IActionResult> ProductCreate(Product product)
        {
            if (ModelState.IsValid)
            {
                Product prod = await _productRepository.SaveProduct(product);
                if (prod != null)
                {
                    TempData["success"] = "Product Created Successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                
            }
            TempData["error"] = "Failed to create Product";
            return View(product);
        }
		[Route("/admin/RemoveItem/{productId:int}")]
		public async Task<IActionResult> RemoveItem(int productId)
		{
            Product? product = await _db.Product.FindAsync(productId);
            try
            {
                _db.Product.Remove(product);
                _db.SaveChanges();
                TempData["success"] = "Product Deleted";
            }
            catch (Exception ex) {
				TempData["error"] = "Failed to Delete product";
			}

			return RedirectToAction("ProductIndex");
		}

		[Route("/admin/orderindex")]
		public async Task<IActionResult> OrderIndex()
		{
			var prders = await _orderRepository.GetOrders();

			return View(prders);
		}

		[Route("/admin/RemoveOrder/{orderId:int}")]
		public async Task<IActionResult> RemoveOrder(int orderId)
		{
			Order? order = await _db.Order.FindAsync(orderId);
			try
			{
				_db.Order.Remove(order);
				_db.SaveChanges();
				TempData["success"] = "Order Deleted";
			}
			catch (Exception ex)
			{
				TempData["error"] = "Failed to Delete order";
			}

			return RedirectToAction("OrderIndex");
		}

        [Route("/admin/EditOrder/{orderId:int}")]
        public async Task<IActionResult> EditOrder(int orderId)
        {
            Order? order = await _db.Order.FindAsync(orderId);
            return View(order);
        }

        
        [HttpPost]
        [Route("/admin/UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            
               
                Order Org = await _db.Order.FindAsync(order.Id);

                
                Org.OrderStatus = order.OrderStatus; 

                // Save the updated order in the repository
               await _db.SaveChangesAsync();

                TempData["success"] = "Order Updated Successfully";
                return RedirectToAction(nameof(OrderIndex));
            
        }

    }
}
