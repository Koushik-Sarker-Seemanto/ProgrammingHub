using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Post;
using Entities;
using LazZiya.ImageResize;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Services.Abstractions;

namespace WebService.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly IPostService _postService;
        private readonly IUploadService _uploadService;
        public PostController(ILogger<PostController> logger, IPostService postService, IUploadService uploadService)
        {
            _logger = logger;
            _postService = postService;
            _uploadService = uploadService;
        }

        [HttpGet]
        public async Task<ActionResult> IndexAsync([FromQuery(Name = "searchKey")] string searchKey = null, [FromQuery(Name = "postType")] string postType = null)
        {
            this._logger.LogInformation($"searchKey: {searchKey}, postType: {postType}");
            var result = await this._postService.GetAllPost(searchKey, postType);
            var response = (List<Post>) result.Data;
            return View(response);
        }
        
        public async Task<ActionResult> Details(string id)
        {
            var result = await this._postService.GetPostById(id);
            if (result.ResponseStatus != ResponseEnum.Ok)
            {
                return View(null);
            }

            var response = (Post) result.Data;
            return View(response);
        }
        
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(IFormFile file, PostModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var authenticationInfo = await HttpContext.AuthenticateAsync();

                if (authenticationInfo != null && authenticationInfo.Succeeded)
                {
                    var userName = authenticationInfo.Principal.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;
                    var uploadSmall = this._uploadService.UploadImage(file, 750, 300);
                    var uploadBig = this._uploadService.UploadImage(file, 900, 300);
                    model.CoverSmall = uploadSmall;
                    model.CoverBig = uploadBig;
                    var result = await this._postService.CreatePost(model, userName);
                    if (result.ResponseStatus != ResponseEnum.Ok)
                    {
                        ModelState.AddModelError("", result.Response);
                        return View(model);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            catch(Exception e)
            {
                this._logger.LogInformation("Error Occurred: ", e);
                return View();
            }
        }


        [HttpGet]
        public async Task<ActionResult> Comment(CommentModel model)
        {
            try
            {
                var authenticationInfo = await HttpContext.AuthenticateAsync();

                if (authenticationInfo != null && authenticationInfo.Succeeded)
                {
                    var userName = authenticationInfo.Principal.Claims.FirstOrDefault(e => e.Type == ClaimTypes.NameIdentifier)?.Value;
                    var result = await this._postService.Comment(model, userName);
                    if (result.ResponseStatus != ResponseEnum.Ok)
                    {
                        ModelState.AddModelError("", result.Response);
                        return Json(new { status = "Fail" });
                    }
                    return Json(new { status = "Ok" });
                }

                return Json(new { status = "Fail" });
            }
            catch
            {
                return Json(new { status = "Fail" });
            }
        }


        // GET: PostController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }

        // GET: PostController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PostController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(IndexAsync));
            }
            catch
            {
                return View();
            }
        }
    }
}
