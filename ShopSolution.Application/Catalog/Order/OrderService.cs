using Microsoft.EntityFrameworkCore;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Catalog.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Catalog.Order
{
    public class OrderService : IOrderService
    {
        private readonly ShopDBContext _context;

        public OrderService(ShopDBContext context)
        {
            _context = context;
        }

        public async Task<List<OrderVm>> GetAll()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                .ToListAsync();

            var orderVms = orders.Select(order => new OrderVm
            {
                Id = order.Id,
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                UserName = order.UserName,
                Email = order.Email,
                Address = order.Address,
                Phone = order.Phone,
                PaymentMethod = order.PaymentMethod,
                Status = (int)order.Status, // Ép kiểu Status sang int
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailVm
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    Price = od.Price
                }).ToList()
            }).ToList();

            return orderVms;
        }
    }
}