using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DnsClient.Internal;
using DomainModels;
using Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Services.Abstractions;

namespace WebService.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> logger;
        private readonly IPostService postService;
        public ProfileController(ILogger<ProfileController> logger, IPostService postService)
        {
            this.logger = logger;
            this.postService = postService;
        }
        // GET: ProfileController
        public async Task<ActionResult> IndexAsync(string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                string currentUserId = HttpContext?.User?.Claims?.FirstOrDefault(e => e.Type == "Id")?.Value;
                var result = await this.postService.GetPostByUserId(currentUserId, true);
                if (result.ResponseStatus == ResponseEnum.Ok)
                {
                    return View(result.Data);
                }
            }
            else
            {
                var result = await this.postService.GetPostByUserId(id, false);
                if (result.ResponseStatus == ResponseEnum.Ok)
                {
                    return View(result.Data);
                }
            }

            return BadRequest();
        }

        // GET: ProfileController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProfileController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfileController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: ProfileController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfileController/Edit/5
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

        // GET: ProfileController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProfileController/Delete/5
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
