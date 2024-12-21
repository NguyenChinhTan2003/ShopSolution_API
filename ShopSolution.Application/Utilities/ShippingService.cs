using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopSolution.Application.Common;
using ShopSolution.Data.EF;
using ShopSolution.Data.Entities;
using ShopSolution.Data.Migrations;
using ShopSolution.Utilities.Exceptions;
using ShopSolution.ViewModels.Catalog.Categories;
using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Roles;
using ShopSolution.ViewModels.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Application.Utilities
{
    public class ShippingService : IShippingService
    {
        private readonly ShopDBContext _context;

        public ShippingService(ShopDBContext context)
        {
            _context = context;
        }

        public async Task<int> Create(ShippingRequest request)
        {
            var ship = new Shipping()
            {
               Price = request.Price,
               Ward = request.Ward,
               District = request.District,
               City = request.City,
            };
            _context.Shippings.Add(ship);
            await _context.SaveChangesAsync();
            return ship.Id;
        }
        public async Task<ShippingVm> GetById(int id)
        {
            var shipping = await _context.Shippings.Where(s => s.Id == id).Select(s => new ShippingVm
        {
            Id = s.Id,
            Price = s.Price,
            Ward = s.Ward,
            District = s.District,
            City = s.City,
            }).FirstOrDefaultAsync();

            return shipping;
        }

        public async Task<List<ShippingVm>> GetAll()
        {
            var shipping = await _context.Shippings
                .Select(x => new ShippingVm()
                {
                    Id = x.Id,
                    City = x.City,
                    District= x.District,
                    Ward= x.Ward,
                    Price= x.Price,
                }).ToListAsync();

            return shipping;
        }

        public async Task<int> Delete(int id)
        {
            var shippings = await _context.Shippings.FindAsync(id);
            if (shippings == null) throw new ShopException($"Cannot find a Shipping: {id}");

           
            _context.Shippings.Remove(shippings);

            return await _context.SaveChangesAsync();
        }
    }
}
