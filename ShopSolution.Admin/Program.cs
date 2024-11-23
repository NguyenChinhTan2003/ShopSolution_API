using Microsoft.AspNetCore.Authentication.Cookies;
using ShopSolution.ApiIntegration;

using ShopSolution.ApiIntegration;

var builder = WebApplication.CreateBuilder(args);

// Thêm d?ch v? vào container
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpClient();

builder.Services.AddTransient<IUserApiClient, UserApiClient>();
builder.Services.AddTransient<ILanguageApiClient, LanguageApiClient>();
builder.Services.AddTransient<IProductApiClient, ProductApiClient>();
builder.Services.AddTransient<ICategoryApiClient, CategoryApiClient>();

builder.Services.AddTransient<IRoleApiClient, RoleApiClient>();

var mvcBuilder = builder.Services.AddRazorPages();

#if DEBUG
// Ch? thêm runtime compilation trong môi tr??ng Development
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}
#endif

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Login/Index/";
                    options.AccessDeniedPath = "/User/Forbidden/";
                });
// Sau khi c?u hình d?ch v? xong, ti?n hành build ?ng d?ng
var app = builder.Build();

// C?u hình pipeline HTTP request
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();
app.UseSession();
// ??nh tuy?n cho Razor Pages và Controllers
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();