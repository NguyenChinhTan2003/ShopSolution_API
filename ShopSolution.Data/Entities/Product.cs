﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public decimal Price { get; set; }

        public int ViewCount { get; set; }

        public List<ProductInCategory> ProductInCategories { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
        public List<Cart> Carts { get; set; }

    }
}
