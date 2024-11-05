using Microsoft.EntityFrameworkCore;
using ShopSolution.Application.Catalog.Products;
using Microsoft.OpenApi.Models;
using ShopSolution.Application.Service.Products;
using ShopSolution.Data.EF;
using ShopSolution.Application.Common;
using ShopSolution.Utilities.Constants;
using Microsoft.AspNetCore.Identity;
using ShopSolution.Application.System.Users;
using ShopSolution.Application.System;
using ShopSolution.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ShopDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString(SystemConstants.MainConnectionString));
});

builder.Services.AddTransient<IStorageService,FileStorageService >();
builder.Services.AddTransient<IPublicProductService, PublicProductService>();
builder.Services.AddTransient<IManageProductService, ManageProductService>();
builder.Services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
builder.Services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
builder.Services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Shop", Version = "v1" });
});
builder.Services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<ShopDBContext>()
                .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Shop V1"));
}

app.MapControllers();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();