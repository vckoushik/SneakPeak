using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SneakPeak.Areas.Identity.Data;
using SneakPeak.Data;
using SneakPeak.Models;
using SneakPeak.Repo;
using SneakPeak.Services;

var builder = WebApplication.CreateBuilder(args);// Builder to add more services to project

var connectionString = builder.Configuration.GetConnectionString("SneakPeakDbContextConnection") ?? throw new InvalidOperationException("Connection string 'SneakPeakDbContextConnection' not found.");

builder.Services.AddDbContext<SneakPeakDbContext>(options => options.UseSqlServer(connectionString));

var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

builder.Services.AddDefaultIdentity<SneakPeakUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<SneakPeakDbContext>().AddDefaultTokenProviders();

builder.Services.AddTransient<ICartRepository, CartRepository>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddTransient<ICartRepository,CartRepository>();
builder.Services.AddTransient<IWishlistRepository, WishlistRepository>();
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<IAddressRepository, AddressRepository>();
builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddTransient<IBraintreeService, BraintreeService>();
builder.Services.AddTransient<IMailService, MailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireNonAlphanumeric=false;
});


var app = builder.Build(); //App to configure project

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseRouting();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});



app.MapRazorPages();
app.Run();
