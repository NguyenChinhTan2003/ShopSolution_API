﻿using ShopSolution.ViewModels.Common;
using ShopSolution.ViewModels.System.Roles;

namespace ShopSolution.ApiIntegration
{
    public interface IRoleApiClient
    {
        Task<ApiResult<List<RoleVm>>> GetAll();
    }
}