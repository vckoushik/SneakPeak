using Microsoft.EntityFrameworkCore;
using SneakPeak.Data;
using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public class ProductRepository : IProductRepository
    {
        private readonly SneakPeakDbContext _db;
        public ProductRepository(SneakPeakDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Product>> GetProducts()
        {
            var products =await _db.Product.ToListAsync();
            return products;
        }

        public async Task<Product> SaveProduct(Product product)
        {
           _db.Product.Add(product);
           int val =await _db.SaveChangesAsync();
           product.Id= val;
           return product;
        }
    }
}
