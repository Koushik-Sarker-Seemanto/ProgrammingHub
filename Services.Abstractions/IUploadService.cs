using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Services.Abstractions
{
    public interface IUploadService
    {
        public string UploadImage(IFormFile file, int height, int width, string rootPath);
    }
}
