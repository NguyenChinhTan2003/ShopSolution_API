using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopSolution.ApiIntegration;
using ShopSolution.ApiIntegration.VnPay;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.Data.Enums;
using ShopSolution.Data.Migrations;
using ShopSolution.Utilities.Constants;
using ShopSolution.ViewModels.Sales;
using ShopSolution.WebApp.Models;
using ShopSolution.WebApp.Models.VnPay;
using System.Net;
using System.Numerics;

namespace ShopSolution.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ShopDBContext _shopDBContext;
        private readonly IVnPayService _vnPayService;
        public CartController(IProductApiClient productApiClient,ShopDBContext shopDBContext,IVnPayService vnPayService)
        {
            _productApiClient = productApiClient;
            _shopDBContext = shopDBContext;
            _vnPayService = vnPayService;
        }
        public IActionResult Index()
        {
            ViewData["ShowSideComponent"] = false;
            return View();
        }

        public IActionResult Checkout()
        {
            ViewData["ShowSideComponent"] = false;
            if (User.Identity.IsAuthenticated){
                return View(GetCheckoutViewModel());
            }


            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Pay(string PaymentMethod,string PaymentId )
        {
            ViewData["ShowSideComponent"] = false;
            // Lấy giỏ hàng từ Session
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            if (session == null)
            {
                TempData["error"] = "Giỏ hàng trống. Không thể thực hiện thanh toán.";
                return View();
            }
            // Tạo một Order mới
            var orderId = Guid.NewGuid();
            var orderItem = new Order();
            orderItem.OrderId = orderId;
            orderItem.OrderDate = DateTime.Now;
            orderItem.UserName = User.Identity.Name;
            if (PaymentMethod != "VnPay")
            {
                orderItem.PaymentMethod = "COD";
            }
            else if (PaymentMethod == "VnPay")
            {
                orderItem.PaymentMethod = "VnPay" + " " + PaymentId;
            }
            orderItem.Email = "";
            orderItem.Phone = "";
            orderItem.Address = "";
            orderItem.Status = 0; // 0 = Đang xử lý
            

           
           
            _shopDBContext.Orders.Add(orderItem);
            _shopDBContext.SaveChanges();

           

            var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);

           
            var orderDetails = cartItems.Select(item => new OrderDetail()
            {
                OrderId = orderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price, 
            }).ToList();

            _shopDBContext.OrderDetails.AddRange(orderDetails);
            _shopDBContext.SaveChanges();

            // Xóa giỏ hàng sau khi đặt hàng thành công
            HttpContext.Session.Remove(SystemConstants.CartSession);

            TempData["success"] = "Đơn hàng đã được tạo thành công!";
            return View(TempData);
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return Redirect(url);
        }

        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            ViewData["ShowSideComponent"] = false;
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VnPayResponseCode == "00")
            {
                var newVnpayInsert = new Vnpay
                {
                    OrderId = response.OrderId,
                    PaymentMethod = response.PaymentMethod,
                    OrderDescription = response.OrderDescription,
                    TotalAmount = response.TotalAmount,
                    TransactionId = response.TransactionId,
                    PaymentId = response.PaymentId,
                    CreatedDate = DateTime.Now,
                };
                _shopDBContext.Add(newVnpayInsert);
                await _shopDBContext.SaveChangesAsync();

                var PaymentMethod = response.PaymentMethod;
                var PaymentId = response.PaymentId;

                await Pay(PaymentMethod, PaymentId);
            }
            return View(response);
        }


        [HttpGet]
        public IActionResult GetListItems()
        {
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
            return Ok(currentCart);
        }

        private CheckoutViewModel GetCheckoutViewModel()
        {
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
            var checkoutVm = new CheckoutViewModel()
            {
                CartItems = currentCart,
                CheckoutModel = new CheckoutRequest()
            };
            return checkoutVm;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, string languageId)
        {
            var product = await _productApiClient.GetById(id, languageId);
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();

            if (session != null)
            {
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
            }

            var existingItem = currentCart.FirstOrDefault(x => x.ProductId == id);
            if (existingItem != null)
            {
                existingItem.Quantity += 1;
            }
            else
            {
                currentCart.Add(new CartItemViewModel
                {
                    ProductId = id,
                    Description = product.Description,
                    Image = product.ThumbnailImage,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = 1
                });
            }

            HttpContext.Session.SetString(SystemConstants.CartSession, JsonConvert.SerializeObject(currentCart));
            return Ok(currentCart);
        }


        public IActionResult UpdateCart(int id, int quantity)
        {
            var session = HttpContext.Session.GetString(SystemConstants.CartSession);
            List<CartItemViewModel> currentCart = new List<CartItemViewModel>();
            if (session != null)
                currentCart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(session);
            foreach (var item in currentCart)
            {
                if (item.ProductId == id)
                {
                    if (quantity == 0)
                    {
                        currentCart.Remove(item);
                        break;
                    }
                    item.Quantity = quantity;
                }
            }
            HttpContext.Session.SetString(SystemConstants.CartSession, JsonConvert.SerializeObject(currentCart));
            return Ok(currentCart);
        }
    }
}
