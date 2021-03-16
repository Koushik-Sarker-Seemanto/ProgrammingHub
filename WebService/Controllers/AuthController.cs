using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DnsClient.Internal;
using DomainModels;
using DomainModels.Auth;
using Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using Services.Abstractions;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace WebService.Controllers
{
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        
        public async Task<ActionResult> LoginAsync()
        {
            var authenticationInfo = await HttpContext.AuthenticateAsync();

            if (authenticationInfo != null && authenticationInfo.Succeeded)
            {
                return RedirectToAction("Index", "Post");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel loginModel)
        {
            // this._logger.LogInformation(JsonConvert.SerializeObject(loginModel));

            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }

            var userResponse = await this._authService.LoginUser(loginModel);
            if (userResponse.ResponseStatus != ResponseEnum.Ok)
            {
                ModelState.AddModelError("", userResponse.Response);
                return View(loginModel);
            }

            var claimsIdentity = this._authService.GetSecurityClaims((User) userResponse.Data, CookieAuthenticationDefaults.AuthenticationScheme);

            var authenticationProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authenticationProperties
            );

            return RedirectToAction("Index", "Post");
        }
        
        public async Task<ActionResult> RegisterAsync()
        {
            var authenticationInfo = await HttpContext.AuthenticateAsync();

            if (authenticationInfo != null && authenticationInfo.Succeeded)
            {
                return RedirectToAction("Index", "Post");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterAsync(RegisterModel registerModel)
        {
            // this._logger.LogInformation(JsonConvert.SerializeObject(registerModel));
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var result = await this._authService.RegisterUser(registerModel);
            if (result.ResponseStatus != ResponseEnum.Ok)
            {
                ModelState.AddModelError("", result.Response);
                return View(registerModel);
            }

            return RedirectToAction("Login", "Auth");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Auth");
        }

        [Authorize]
        public ActionResult AuthorizedPage()
        {
            return View();
        }
        
        public ActionResult UnauthorizedPage()
        {
            return View();
        }
    }
}
