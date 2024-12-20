﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.Utilities.Constants
{
    public class SystemConstants
    {
        public const string MainConnectionString = "DefaultConnection";
        public const string CartSession = "CartSession";

        public class AppSettings
        {
            public const string DefaultLanguageId = "DefaultLanguageId";
            public const string Token = "Token";
            public const string BaseAddress = "BaseAddress";
        }

        public class ProductSettings
        {
            public const int NumberOfFeaturedProducts = 8;
            public const int NumberOfLastedProducts = 8;
        }

        public class ProductConstants
        {
            public const string NA = "N/A";
        }
    }
}