using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ShopSolution.Application.Catalog.Products;
using ShopSolution.Application.Common;
using ShopSolution.Application.Service.Products;
using ShopSolution.Application.System;
using ShopSolution.Application.System.Users;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.Utilities.Constants;
using FluentValidation;
using ShopSolution.ViewModels.System.Users;
using ShopSolution.Application.System.Roles;
using ShopSolution.Application.System.Languages;
using ShopSolution.Application.Catalog.Categories;
using ShopSolution.Application.Utilities.Slide;
using Hangfire;
using ShopSolution.BackendApi;
using Hangfire.MemoryStorage;
using ShopSolution.BackendApi.Services;

var builder = WebApplication.CreateBuilder(args);

//Add services to the container.
builder.Services.AddControllers()
    .AddMvcOptions(options =>
    {
        // T?t AutoValidateAntiforgeryToken n?u b?n không c?n
        options.Filters.Clear();
    });

builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

//builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
//builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();

// Thêm CORS n?u c?n
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// DB Context
builder.Services.AddDbContext<ShopDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString(SystemConstants.MainConnectionString));
});

// Identity configuration
builder.Services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<ShopDBContext>()
                .AddDefaultTokenProviders();

// DI Registration
builder.Services.AddTransient<IStorageService, FileStorageService>();
builder.Services.AddTransient<IProductService, ProductService>();
builder.Services.AddTransient<UserManager<AppUser>, UserManager<AppUser>>();
builder.Services.AddTransient<SignInManager<AppUser>, SignInManager<AppUser>>();
builder.Services.AddTransient<RoleManager<AppRole>, RoleManager<AppRole>>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IRoleService, RoleService>();
builder.Services.AddTransient<ILanguageService, LanguageService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<ISlideService, SlideService>();

// JWT Configuration
string? issuer = builder.Configuration["Tokens:Issuer"];
string? signingKey = builder.Configuration["Tokens:Key"];

if (string.IsNullOrEmpty(issuer))
    throw new InvalidOperationException("JWT issuer configuration is missing");

if (string.IsNullOrEmpty(signingKey))
    throw new InvalidOperationException("JWT signing key configuration is missing");

byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = issuer,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
    };
});

// Swagger Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Shop", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});



// Đọc thông tin đăng nhập từ configuration
var dashboardUsername = builder.Configuration["Hangfire:DashboardUsername"];
var dashboardPassword = builder.Configuration["Hangfire:DashboardPassword"];

// Cấu hình Hangfire với MemoryStorage
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseMemoryStorage());

builder.Services.AddHangfireServer();

// Đăng ký service
builder.Services.AddScoped<IJobService, JobService>();


var app = builder.Build();


// Cấu hình Hangfire Dashboard với Basic Authentication
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new HangfireDashboardAuthorizationFilter(
            dashboardUsername ?? "admin",
            dashboardPassword ?? "Admin1@34"
        )
    }
});


using (var serviceProvider = app.Services.CreateScope())
{
    var jobService = serviceProvider.ServiceProvider.GetRequiredService<IJobService>();

    RecurringJob.AddOrUpdate(
        "SendEmail",
        () => jobService.SendEmail(),
        "* 9 * * 1");

    RecurringJob.AddOrUpdate(
        "SendEmail2",
        () => jobService.SendEmail(),
        "0 9 1 * *");

    RecurringJob.AddOrUpdate(
        "UpdateIsFeatured",
        () => jobService.UpdateIsFeatured(),
        "*/1 * * * *");


}

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Shop V1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Use CORS if configured
app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();