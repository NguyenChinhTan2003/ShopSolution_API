using ShopSolution.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.System.Users
{
    public class GetUserPagingRequest : PagingRequestBase
    {
        public string? Keyword { get; set; }
    }
}