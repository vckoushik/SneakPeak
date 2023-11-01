using SneakPeak.Models;

namespace SneakPeak.Repo
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> SaveProduct(Product product);

    }
}
