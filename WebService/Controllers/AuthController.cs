using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.AspNetCore.Hosting;
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
        private readonly IUploadService _uploadService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(ILogger<AuthController> logger, IAuthService authService, IUploadService uploadService, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _authService = authService;
            _uploadService = uploadService;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<ActionResult> RegisterAsync(IFormFile file, RegisterModel registerModel)
        {
            // this._logger.LogInformation(JsonConvert.SerializeObject(registerModel));
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }

            var rootPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            var profileImage = this._uploadService.UploadImage(file, 512, 512, rootPath);
            registerModel.ProfileImage = profileImage;
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
