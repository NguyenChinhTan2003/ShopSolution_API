﻿using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.Catalog.Products
{
    public class GetManageProductPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }

        public int? CategoryId { get; set; }
        public string LanguageId { get; set; }
    }
}
