using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Data;
using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public class CartRepository : ICartRepository
    {
        private readonly SneakPeakDbContext _context;
        private readonly UserManager<SneakPeakUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartRepository(SneakPeakDbContext context, UserManager<SneakPeakUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = contextAccessor;

        }
        public async Task<int> AddItem(int productId, int quantity,string size)
        {
            string userId = GetUserId();
            var transaction = _context.Database.BeginTransaction();
            try {
                
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Invalid user id ");
                }
                var cart = await GetCart(userId);
                if (cart == null) {
                    cart = new Cart()
                    {
                        UserId = userId
                    };
                    _context.Cart.Add(cart);
                }
                _context.SaveChanges();
                //For existing cart if we have a item in cart
                var cartItem = _context.CartItem.FirstOrDefault(a => a.CartId == cart.Id && a.ProductId == productId);
                if (cartItem is not null) {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    var product = _context.Product.Find(productId);
                    cartItem = new CartItem() {
                        ProductId = productId, CartId = cart.Id,
                        Quantity = quantity,
                        Product = product,
                        Size= size,
                        PricePerUnit = product.Price 
                    };
                    _context.CartItem.Add(cartItem);
                }
                _context.SaveChanges();
                transaction.Commit();
               

            }
            catch (Exception ex) {
            }
            var totalItems =await GetCartItemsCount(userId);
            return totalItems;

        }

        public async Task<int> RemoveItem(int productId)
        {
            string userId = GetUserId();
            try
            {
                
                if (string.IsNullOrEmpty(userId))
                {
                    throw new Exception("Invalid user id");
                }
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    throw new Exception("Invalid cart");
                }
                _context.SaveChanges();
                //For existing cart if we have a item in cart
                var cartItem = _context.CartItem.FirstOrDefault(a => a.CartId == cart.Id && a.ProductId == productId);
                if (cartItem is null)
                {
                    throw new Exception("No Items in Cart ");
                }
                else if (cartItem.Quantity == 1)
                {
                    _context.CartItem.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity -= 1;

                }
                _context.SaveChanges();
                

            }
            catch (Exception ex)
            {
            }
            var totalItems = await GetCartItemsCount(userId);
            return totalItems;

        }

        public async Task<Cart> GetUserCart(){
            var userId = GetUserId();
            if(userId == null)
            {
                throw Exception("invalid user");
            }
            var cart = await _context.Cart.Include(a=>a.Items)
                                           .ThenInclude(a=>a.Product)
                                           .Where(a=>a.UserId== userId).FirstOrDefaultAsync();

            return cart;


        }

        public async Task<int> GetCartItemsCount(string userId = "")
        {
            if (string.IsNullOrEmpty(userId)){
                userId=GetUserId(); 
            }
            var data = await (from cart in _context.Cart
                              join cartItem in _context.CartItem
                              on cart.Id equals cartItem.CartId
                              where cart.UserId == userId
                              select new { cartItem.Id }).ToListAsync();
            return data.Count;
        }

        public async Task<bool> DoCheck()
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                var userId = GetUserId();
                if(userId == null ) {
                    throw new Exception("User not logged In");
                }
                var cart = await GetCart(userId);
                if(cart is null)
                {
                    throw new Exception("Invalid Cart");
                }
                var cartItems=await _context.CartItem.Where(a=>a.CartId == cart.Id).ToListAsync();
                if(cartItems.Count == 0)
                {
                    throw new Exception("Cart is Empty");
                }

                var order = new Order
                {
                    UserId = userId,
                    CreatedDate = DateTime.Now,
                    OrderStatus = "Pending",

                };
                _context.Order.Add(order);
                _context.SaveChanges();
                foreach(var item in cartItems)
                {
                    var orderLineItem = new OrderLineItem
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        PricePerUnit = item.PricePerUnit,
                    };
                    _context.OrderLineItem.Add(orderLineItem);
                   
                }
                _context.SaveChanges();

                //Remove from cart
                _context.CartItem.RemoveRange(cartItems);
                _context.SaveChanges(); 
                transaction.Commit();
                return true;

            }
            catch (Exception ex) {
                return false;
            }
        }

        private Exception Exception(string v)
        {
            throw new NotImplementedException();
        }

        private async Task<Cart> GetCart(string userId)
        {
            var result = _context.Cart.FirstOrDefault(x=>x.UserId == userId); 
            return result;
        }

        private string GetUserId()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userId= _userManager.GetUserId(user);
            return userId;

        }
        public async Task<string> GetUserEmailAsync()
        {
            var user = _httpContextAccessor.HttpContext.User;
            var userId = _userManager.GetUserId(user);
            var usr = await _userManager.GetUserAsync(user);
            var email = usr?.Email;

            return email;

        }
    }

}
