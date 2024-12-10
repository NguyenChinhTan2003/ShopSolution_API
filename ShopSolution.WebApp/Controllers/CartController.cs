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
using ShopSolution.ViewModels.System.Languages;
using ShopSolution.WebApp.Models;
using ShopSolution.WebApp.Models.VnPay;
using ShopSolution.WebApp.Service;
using System.Net;
using System.Numerics;

namespace ShopSolution.WebApp.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ShopDBContext _shopDBContext;
        private readonly IVnPayService _vnPayService;
        private readonly IEmailSender _emailSender;
        public CartController(IProductApiClient productApiClient,ShopDBContext shopDBContext,IVnPayService vnPayService, IEmailSender emailSender)
        {
            _productApiClient = productApiClient;
            _shopDBContext = shopDBContext;
            _vnPayService = vnPayService;
            _emailSender = emailSender;
        }
        private string GenerateOrderDetailsHtml(Order order, List<OrderDetail> orderDetails, List<ProductTranslation> productTranslations, string languageId)
        {
            // Nếu phương thức thanh toán chưa được đặt, mặc định là "COD"
            if (string.IsNullOrEmpty(order.PaymentMethod))
            {
                order.PaymentMethod = "COD";
            }

            var orderDetailsHtml = string.Join("", orderDetails.Select(detail =>
            {
                var productTranslation = productTranslations
                    .FirstOrDefault(pt => pt.ProductId == detail.ProductId && pt.LanguageId == languageId);
                var productName = productTranslation?.Name ?? "Không xác định";

                return $@"
                <tr>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{detail.ProductId}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{productName}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{detail.Quantity}</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{detail.Price.ToString("N0")} VNĐ</td>
                    <td style='border: 1px solid #ddd; padding: 8px;'>{(detail.Quantity * detail.Price).ToString("N0")} VNĐ</td>
                </tr>";
                            }));

                            var html = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6;'>
                    <h2 style='color: #4CAF50;'>Cảm ơn bạn đã đặt hàng!</h2>
                    <p>Chi tiết đơn hàng của bạn như sau:</p>
                    <table style='width: 100%; border-collapse: collapse; margin-top: 20px;'>
                        <thead>
                            <tr style='background-color: #f2f2f2;'>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Mã sản phẩm</th>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Tên sản phẩm</th>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Số lượng</th>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Đơn giá</th>
                                <th style='border: 1px solid #ddd; padding: 8px;'>Thành tiền</th>
                            </tr>
                        </thead>
                        <tbody>
                            {orderDetailsHtml}
                        </tbody>
                    </table>
                    <h3 style='margin-top: 20px;'>Tổng tiền: {orderDetails.Sum(d => d.Quantity * d.Price).ToString("N0")} VNĐ</h3>
                    <p><strong>Phương thức thanh toán:</strong> {order.PaymentMethod}</p>
                    <p><strong>Ngày đặt hàng:</strong> {order.OrderDate.ToString("dd/MM/yyyy HH:mm")}</p>
                </div>";

            return html;
        }



        public IActionResult Index()
        {
            ViewData["ShowSideComponent"] = false;
            return View();
        }

        public IActionResult Checkout()
        {
            ViewData["ShowSideComponent"] = false;
            if (User.Identity.IsAuthenticated)
            {
                return View(GetCheckoutViewModel());
            }


            return RedirectToAction("Login", "Account");
        }

        public async Task<IActionResult> Pay(string PaymentMethod, string PaymentId, string languageId = "vi")
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
            var orderItem = new Order
            {
                OrderId = orderId,
                OrderDate = DateTime.Now,
                UserName = User.Identity.Name,
                PaymentMethod = PaymentMethod == "VnPay" ? $"VnPay {PaymentId}" : "COD",
                Email = "",
                Phone = "",
                Address = "",
                Status = 0 // 0 = Đang xử lý
                
            };



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

            var productTranslations = _shopDBContext.ProductTranslations.ToList();
            // Xóa giỏ hàng sau khi đặt hàng thành công
            HttpContext.Session.Remove(SystemConstants.CartSession);

            var emailContent = GenerateOrderDetailsHtml(orderItem, orderDetails, productTranslations, languageId);

            var receiver = User.Identity.Name;
            var subject = "Đặt đơn hàng thành công!";

            await _emailSender.SendEmailAsync(receiver, subject, emailContent);


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
