using Microsoft.AspNetCore.Authentication.Cookies;
using ShopSolution.ApiIntegration;

using ShopSolution.ApiIntegration;

var builder = WebApplication.CreateBuilder(args);

// Th�m d?ch v? v�o container
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
// Ch? th�m runtime compilation trong m�i tr??ng Development
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
// Sau khi c?u h�nh d?ch v? xong, ti?n h�nh build ?ng d?ng
var app = builder.Build();

// C?u h�nh pipeline HTTP request
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
// ??nh tuy?n cho Razor Pages v� Controllers
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();