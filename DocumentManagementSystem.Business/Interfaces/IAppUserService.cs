﻿using DocumentManagementSystem.Common;
using DocumentManagementSystem.Dtos;
using DocumentManagementSystem.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DocumentManagementSystem.Business.Interfaces
{
    public interface IAppUserService : IService<AppUserCreateDto, AppUserUpdateDto, AppUserListDto, AppUser>
    {
        Task<IResponse<AppUserCreateDto>> CreateWithRoleAsync(AppUserCreateDto dto, int roleId);
        Task<IResponse<AppUserListDto>> CheckUserAsync(AppUserLoginDto dto);
        Task<IResponse<List<AppRoleListDto>>> GetRolesByUserIdAsync(int userId);
        Task<IResponse<AppUserListDto>> GetUserByIdAsync(int userId);
    }
}
