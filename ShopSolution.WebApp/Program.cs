using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopSolution.ApiIntegration;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Login/";
                    options.AccessDeniedPath = "/User/Forbidden/";
                });

builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<ShopDBContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped<SignInManager<AppUser>>();
builder.Services.AddScoped<UserManager<AppUser>>();

builder.Services.AddScoped<IUserApiClient,UserApiClient>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddDbContext<ShopDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Auth:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Auth:Google:ClientSecret"];
    })
    .AddFacebook(facebookOptions => {
        facebookOptions.ClientId = builder.Configuration["Auth:Facebook:AppId"];
        facebookOptions.ClientSecret = builder.Configuration["Auth:Facebook:AppSecret"];
        facebookOptions.Scope.Add("email");
    });
// Thêm c?u hình session tr??c khi g?i builder.Build()
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Thêm session vào middleware pipeline
app.UseSession();

app.UseAuthentication(); // Thêm UseAuthentication tr??c UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
