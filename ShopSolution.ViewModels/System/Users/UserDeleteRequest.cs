using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSolution.ViewModels.System.Users
{
<<<<<<<< HEAD:ShopSolution.ViewModels/System/Users/UserDeleteRequest.cs
    public class UserDeleteRequest
    {
        public Guid Id { get; set; }
========
    public class ApiResult<T>
    {
        public bool IsSuccessed { get; set; }

        public string? Message { get; set; }

        public T? ResultObj { get; set; }
>>>>>>>> 2491a6735c13845e75e7fd8b7eff1f18ce1a13cf:ShopSolution.ViewModels/Common/ApiResult.cs
    }
}