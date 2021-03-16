using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Auth;
using Entities;

namespace Services.Abstractions
{
    public interface IAuthService
    {
        Task<ResponseModel> LoginUser(LoginModel model);
        Task<ResponseModel> RegisterUser(RegisterModel model);
        public ClaimsIdentity GetSecurityClaims(User userInfo, string authenticationType = null);
    }
}
