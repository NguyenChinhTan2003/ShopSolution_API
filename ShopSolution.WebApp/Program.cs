using LazZiya.ExpressLocalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using ShopSolution.ApiIntegration;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.WebApp.LocalizationResources;
using System.Globalization;
using ShopSolution.ApiIntegration.VnPay;

var builder = WebApplication.CreateBuilder(args);

// Thêm d?ch v? vào container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login/";
        options.AccessDeniedPath = "/User/Forbidden/";
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Auth:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.ClientId = builder.Configuration["Auth:Facebook:AppId"];
        facebookOptions.ClientSecret = builder.Configuration["Auth:Facebook:AppSecret"];
        facebookOptions.Scope.Add("email");
    });

builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ShopDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddTransient<ISlideApiClient, SlideApiClient>();
builder.Services.AddTransient<IProductApiClient, ProductApiClient>();
builder.Services.AddTransient<ICategoryApiClient, CategoryApiClient>();
builder.Services.AddScoped<SignInManager<AppUser>>();
builder.Services.AddScoped<UserManager<AppUser>>();
builder.Services.AddScoped<IUserApiClient, UserApiClient>();
builder.Services.AddScoped<IVnPayService, VnPayService>();


builder.Services.AddDbContext<ShopDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var cultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("vi"),
};

builder.Services.AddControllersWithViews()
    .AddExpressLocalization<ExpressLocalizationResource, ViewLocalizationResource>(ops =>
    {
        ops.UseAllCultureProviders = false;
        ops.ResourcesPath = "LocalizationResources";
        ops.RequestLocalizationOptions = options =>
        {
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
            options.DefaultRequestCulture = new RequestCulture("vi");
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware c? b?n
app.UseHttpsRedirection();
app.UseStaticFiles();

// Thêm Session và Localization tr??c Authentication
app.UseSession();
app.UseRequestLocalization();

// Thêm Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
     pattern: "{culture=vi}/{controller=Home}/{action=Index}/{id?}");

// Map các route ph? thu?c ngôn ng?
app.MapControllerRoute(
    name: "Product Category En",
    pattern: "{culture=en}/categories/{id}",
    defaults: new { controller = "Product", action = "Category" });
app.MapControllerRoute(
    name: "Product Category Vn",
    pattern: "{culture=vi}/danh-muc/{id}",
    defaults: new { controller = "Product", action = "Category" });
app.MapControllerRoute(
    name: "Product Detail En",
    pattern: "{culture=en}/products/{id}",
    defaults: new { controller = "Product", action = "Detail" });
app.MapControllerRoute(
    name: "Product Detail Vn",
    pattern: "{culture=vi}/san-pham/{id}",
    defaults: new { controller = "Product", action = "Detail" });

app.Run();
