using Microsoft.AspNetCore.Authentication.Cookies;
using ShopSolution.ApiIntegration;

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

builder.Services.AddScoped<IUserApiClient,UserApiClient>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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
