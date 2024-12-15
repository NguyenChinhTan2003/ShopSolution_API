using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ShopSolution.ApiIntegration;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.System.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using ShopSolution.Data.Entities;
using System.Data;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.IO;
using Azure.Core;
using ShopSolution.WebApp.Service;
using ShopSolution.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace ShopSolution.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ShopDBContext _shopDBContext;

        string appid = string.Empty;
        string appsecret = string.Empty;

        public AccountController(IUserApiClient userApiClient,
            IConfiguration configuration, SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager, IEmailSender emailSender, ShopDBContext shopDBContext)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _shopDBContext = shopDBContext;

          
            appid = configuration.GetSection("AppID").Value;
            appsecret = configuration.GetSection("AppSecret").Value;
            

        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var loginRequest = new LoginRequest()
            {
                Schemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
            };
            
            return View(loginRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
                return View();

            // Kiểm tra thông tin đăng nhập trực tiếp
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                return View(request);
            }

            var result = await _signInManager.PasswordSignInAsync(
                request.UserName,
                request.Password,
                request.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Thực hiện API authentication để lấy token nếu cần
                var token = await _userApiClient.Authenticate(request);
                if (!string.IsNullOrEmpty(token.ResultObj))
                {
                    HttpContext.Session.SetString(SystemConstants.AppSettings.Token, token.ResultObj);
                }

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("FirstName", user.FirstName ?? ""),
                        new Claim("LastName", user.LastName ?? "")
                    };

             
                await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);

                return RedirectToAction("Index", "Home");
        }
            else
            {
                ModelState.AddModelError("", "Đăng nhập không đúng.");
                return View(request);
            }
        }

        public async Task<IActionResult> History()
        {
            ViewData["ShowSideComponent"] = false;
            ViewData["ShowSideBar"] = false;

            if ((bool)!User.Identity?.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _shopDBContext.Orders
             .Where(od => od.UserName == userEmail)
             .OrderByDescending(od => od.OrderId)
             .ToListAsync();

            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }

        public async Task<IActionResult> CancenlOrder(Guid orderId)
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Tìm đơn hàng theo orderId
                var order = await _shopDBContext.Orders
                    .Where(o => o.OrderId == orderId)
                    .FirstOrDefaultAsync();

                if (order == null)
                {
                    return NotFound("Order not found.");
                }

                // Cập nhật trạng thái đơn hàng
                order.Status = Data.Enums.OrderStatus.Canceled;
                _shopDBContext.Update(order);

                // Lấy danh sách OrderDetails tương ứng với đơn hàng
                var orderDetails = await _shopDBContext.OrderDetails
                    .Where(od => od.OrderId == orderId)
                    .ToListAsync();

                foreach (var detail in orderDetails)
                {
                    // Tìm sản phẩm trong bảng Product
                    var product = await _shopDBContext.Products
                        .Where(p => p.Id == detail.ProductId)
                        .FirstOrDefaultAsync();

                    if (product != null)
                    {
                        // Cộng lại số lượng vào Stock
                        product.Stock += detail.Quantity;
                        product.Sold -= detail.Quantity;
                        _shopDBContext.Update(product);
                    }
                }

                // Lưu thay đổi vào database
                await _shopDBContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction("History", "Account");
        }


        public async Task<IActionResult> ViewOrder(Guid orderId)
        {
            ViewData["ShowSideComponent"] = false;
            ViewData["ShowSideBar"] = false;

            var detailOrder = await _shopDBContext.OrderDetails.Include(o => o.Product).Where(o => o.OrderId == orderId).ToListAsync();
            return View(detailOrder);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            if (HttpContext.Session.Keys.Contains("Token"))
            {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Remove("Token");
            }

            return RedirectToAction("Index", "Home"); 
        }


        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            SecurityToken validatedToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();

            validationParameters.ValidateLifetime = true;

            validationParameters.ValidAudience = _configuration["Tokens:Issuer"];
            validationParameters.ValidIssuer = _configuration["Tokens:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out validatedToken);

            return principal;
        }

        public IActionResult ExternalLogin(string provider, string returnUrl = "")
        {
            if (provider == "Google")
            {
                var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });

                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

                return new ChallengeResult(provider, properties);
            }
            else if (provider == "Facebook")
            {
                return FacebookLogin(returnUrl); // Chuyển đến hàm FacebookLogin
            }

            ModelState.AddModelError("", "Provider không được hỗ trợ.");
            return RedirectToAction("Login");
           
        }
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "", string remoteError = "")
        {

            var loginRequest = new LoginRequest()
            {
                Schemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
            };

            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError("", $"Error from extranal login provide: {remoteError}");
                return View(loginRequest);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", $"Error from extranal login provide: {remoteError}");
                return View(loginRequest);
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded) {
                var tokenRequest = new LoginRequest
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email) ?? info.Principal.FindFirstValue(ClaimTypes.Name),
                    Password = string.Empty // Password không cần thiết với login external
                };

                var token = await _userApiClient.Authenticate(tokenRequest);
                if (!string.IsNullOrEmpty(token.ResultObj))
                {
                    HttpContext.Session.SetString(SystemConstants.AppSettings.Token, token.ResultObj);
                }
                return RedirectToAction("Index", "Home");
            }
               
            else
            {
                var userEmail = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var user = await _userManager.FindByEmailAsync(userEmail);
                    var name = info.Principal.Identity.Name;

                    if (user == null)
                    {
                        user = new AppUser()
                        {
                            UserName = userEmail,
                            Email = userEmail,
                            EmailConfirmed = true,
                            Dob = DateTime.Now,
                            FirstName = "",
                            LastName = name,

                        };

                        await _userManager.CreateAsync(user);

                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("FirstName", user.FirstName ?? ""),
                        new Claim("LastName", user.LastName ?? "")
                    };

                    var receiver = user.Email;
                    var subject = "Tạo tài khoản thành công!";
                    var message = "Tài khoản của quý khác đã được tạo, chúc quý khác sử dụng dịch vụ vui vẻ!";
                    await _emailSender.SendEmailAsync(receiver, subject, message);

                    var tokenRequest = new LoginRequest
                    {
                        UserName = userEmail,
                        Password = string.Empty // Password không cần thiết với login external
                    };

                    var token = await _userApiClient.Authenticate(tokenRequest);
                    if (!string.IsNullOrEmpty(token.ResultObj))
                    {
                        HttpContext.Session.SetString(SystemConstants.AppSettings.Token, token.ResultObj);
                    }
                    await _signInManager.SignInWithClaimsAsync(user, isPersistent: false, claims);


                    return RedirectToAction("Index", "Home");
                }

            }

            ModelState.AddModelError("", $"Something went wrong");
            return View(loginRequest);
        }


        public IActionResult FacebookLogin( string returnUrl = "")
        {
            var redirectUrl = Url.Action("FacebookLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
            return new ChallengeResult("Facebook", properties);
        }



        public async Task<IActionResult> FacebookLoginCallback(string returnUrl = "", string remoteError = "")
        {
            var loginRequest = new LoginRequest()
            {
                Schemes = await _signInManager.GetExternalAuthenticationSchemesAsync()
            };

            if (!string.IsNullOrEmpty(remoteError))
            {
                ModelState.AddModelError("", $"Error from external login provider: {remoteError}");
                return View(loginRequest);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError("", "Error loading external login information.");
                return View(loginRequest);
            }

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

           

            if (user == null)
            {

                user = new AppUser
                {
                    UserName = info.ProviderKey,
                   
                    Dob = DateTime.Now,
                    FirstName = info.Principal.Identity.Name.Split(' ')[0],
                    LastName = info.Principal.Identity.Name.Split(' ')[1]
                };

             
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user, info);
            }
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim("FirstName", user.FirstName ?? ""),
                        new Claim("LastName", user.LastName ?? "")
                    };
            var token = await _userApiClient.Authenticate(loginRequest);
            if (!string.IsNullOrEmpty(token.ResultObj))
            {
                HttpContext.Session.SetString(SystemConstants.AppSettings.Token, token.ResultObj);
            }

            await _signInManager.SignInWithClaimsAsync(user, isPersistent: false,claims);
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(registerRequest);
            }
            var user = await _userManager.FindByNameAsync(registerRequest.UserName);
            if (user != null)
            {
                ModelState.AddModelError("UserName", "Tài khoản đã tồn tại.");
                return View(ModelState);
            }

            // Tạo đối tượng người dùng mới từ thông tin đăng ký
            var newUser = new AppUser
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                EmailConfirmed = true,
                FirstName = registerRequest.FirstName,
                LastName = registerRequest.LastName,
                Dob = registerRequest.Dob,
            };

            // Tạo tài khoản người dùng với mật khẩu
            var result = await _userManager.CreateAsync(newUser, registerRequest.Password);
           
            if (result.Succeeded)
            {

                // Gán vai trò mặc định (nếu có)
                //await _userManager.AddToRoleAsync(newUser, "User");

                // đăng ký thành công
                // trả về trang login   
                var receiver = registerRequest.Email;
                var subject = "Tạo tài khoản thành công!";
                var message = "Tài khoản của quý khác đã được tạo, chúc quý khác sử dụng dịch vụ vui vẻ!";

                await _emailSender.SendEmailAsync(receiver, subject, message);

                return RedirectToAction("Login", "Account");

            }

            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(registerRequest);
            }
        }

    }
}
