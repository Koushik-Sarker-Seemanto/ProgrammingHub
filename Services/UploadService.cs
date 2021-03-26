using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using LazZiya.ImageResize;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Services.Abstractions;

namespace Services
{
    public class UploadService : IUploadService
    {
        private readonly ILogger<UploadService> _logger;
        public UploadService(ILogger<UploadService> logger)
        {
            _logger = logger;
        }

        public string UploadImage(IFormFile file, int height, int width)
        {
            if (file.Length > 0)
            {
                this._logger.LogInformation($"UploadImage Started");
                Image uploadedImage;
                using (var stream = file.OpenReadStream())
                {
                    uploadedImage = Image.FromStream(stream);
                }
                string phrase = file.FileName;
                string[] words = phrase.Split('.');
                var img = this.ScaleImage(uploadedImage, height, width);

                var imgPath = Guid.NewGuid().ToString() + "." + words[1];

                this._logger.LogInformation($"Going to save image at wwwroot/images/{ imgPath}");
                img.SaveAs($"wwwroot/images/{imgPath}");
                return imgPath;

            }

            return "";
        }

        private Image ScaleImage(Image image, int maxHeight, int maxWeight)
        {
            this._logger.LogInformation("ScaleImage Started");
            var newImage = new Bitmap(maxHeight, maxWeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(image, 0, 0, maxHeight, maxWeight);
            }
            return newImage;
        }
    }
}
