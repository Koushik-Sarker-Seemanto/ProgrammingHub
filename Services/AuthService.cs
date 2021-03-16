using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Auth;
using Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using Repository;
using Services.Abstractions;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<AuthService> _logger;
        public AuthService(IMongoRepository repository, ILogger<AuthService> logger)
        {
            this._repository = repository;
            _logger = logger;
        }

        public async Task<ResponseModel> LoginUser(LoginModel model)
        {
            ResponseModel response = new ResponseModel();
            var user = await this._repository.GetItemAsync<User>(e => e.Username == model.Username);
            if (user == null)
            {
                response.Response = "Invalid User";
                response.ResponseStatus = ResponseEnum.UserDoesNotExist;
                return response;
            }
            var passResult = new PasswordHasher().VerifyHashedPassword(user.Password, model.Password);
            if (passResult.Equals(PasswordVerificationResult.Failed))
            {
                response.ResponseStatus = ResponseEnum.InvalidPassword;
                response.Response = "Invalid Password";
                return response;
            }

            user.Password = null;
            response.ResponseStatus = ResponseEnum.Ok;
            response.Response = "Ok";
            response.Data = user;
            return response;
        }

        public async Task<ResponseModel> RegisterUser(RegisterModel model)
        {
            ResponseModel response = new ResponseModel();

            var usernameExist = await this._repository.GetItemAsync<User>(e => e.Username == model.Username);
            if (usernameExist != null)
            {
                response.Response = "Username Already Exists";
                response.ResponseStatus = ResponseEnum.AlreadyExist;
                return response;
            }

            var phoneExist = await this._repository.GetItemAsync<User>(e => e.Phone == model.Phone);
            if (phoneExist != null)
            {
                response.Response = "Phone Already Exists";
                response.ResponseStatus = ResponseEnum.AlreadyExist;
                return response;
            }

            var emailExist = await this._repository.GetItemAsync<User>(e => e.Email == model.Email);
            if (emailExist != null)
            {
                response.Response = "Email Already Exists";
                response.ResponseStatus = ResponseEnum.AlreadyExist;
                return response;
            }

            var password = new PasswordHasher().HashPassword(model.Password);

            User user = new User
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Email = model.Email,
                Password = password,
                Gender = model.Gender,
                Phone = model.Phone,
                Username = model.Username,
                CreatedAt = DateTime.Now,
            };

            try
            {
                await this._repository.SaveAsync<User>(user);
                response.ResponseStatus = ResponseEnum.Ok;
                response.Response = "Ok";
                return response;
            }
            catch (Exception e)
            {
                this._logger.LogError($"Exception while saving user to DB: {e.Message}");
                response.ResponseStatus = ResponseEnum.InternalError;
                return response;
            }
        }

        public ClaimsIdentity GetSecurityClaims(User userInfo, string authenticationType = null)
        {
            try
            {
                if (userInfo == null)
                {
                    return null;
                }

                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userInfo.Username),
                    new Claim(ClaimTypes.Name, $"{userInfo.Name}"),
                    new Claim("Name", $"{userInfo.Name}"),
                    new Claim("Username", $"{userInfo.Username}"),
                    new Claim("Id", userInfo.Id.ToString()),
                    /*new Claim("Role", $"{userInfo.Role}"),*/
                    new Claim("UserData", JsonConvert.SerializeObject(userInfo)),
                };

                if (!string.IsNullOrEmpty(authenticationType))
                {
                    return new ClaimsIdentity(claims, authenticationType);
                }

                return new ClaimsIdentity(claims);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, $"GetSecurityClaims Failed {ex.Message}");
            }

            return null;
        }
    }
}
