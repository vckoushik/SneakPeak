using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Models;

namespace SneakPeak.Data;

public class SneakPeakDbContext : IdentityDbContext<SneakPeakUser>
{
    public SneakPeakDbContext(DbContextOptions<SneakPeakDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
       
    }

    public DbSet<SneakPeak.Models.Product> Product { get; set; } = default!;
    public DbSet<Order> Order { get; set; }
    public DbSet<OrderLineItem> OrderLineItem { get; set; }

    public DbSet<Cart> Cart { get; set; }
    public DbSet<Wishlist> Wishlist { get; set; }
    public DbSet<WishlistItems> WishlistItems { get; set; }
    public DbSet<CartItem> CartItem { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Address> Address { get; set; }
}
