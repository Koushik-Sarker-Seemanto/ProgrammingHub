using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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

        public string UploadImage(IFormFile file, int height, int width, string rootPath)
        {
            try
            {
                if (file.Length > 0)
                {
                    this._logger.LogInformation($"UploadImage Started");
                    Image uploadedImage;
                    using (var stream = file.OpenReadStream())
                    {
                        this._logger.LogInformation($"UploadImage OpenReadStream");
                        uploadedImage = Image.FromStream(stream);
                    }
                    string phrase = file.FileName;
                    this._logger.LogInformation($"UploadImage FileName: {phrase}");
                    string[] words = phrase.Split('.');
                    var img = this.ScaleImage(uploadedImage, height, width);

                    var imgPath = Guid.NewGuid().ToString() + "." + words[1];

                    this._logger.LogInformation($"Going to save image at wwwroot/images/{ imgPath}");
                    var path = Path.Combine(rootPath, imgPath);
                    img.SaveAs($"{path}");
                    return imgPath;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"UploadImage Exception: {e}");
                throw;
            }

            return "";
        }

        private Image ScaleImage(Image image, int maxHeight, int maxWeight)
        {
            this._logger.LogInformation("ScaleImage Started");
            var newImage = new Bitmap(maxHeight, maxWeight);
            using (var g = Graphics.FromImage(newImage))
            {
                this._logger.LogInformation("ScaleImage Drawing Image");
                g.DrawImage(image, 0, 0, maxHeight, maxWeight);
            }
            return newImage;
        }
    }
}
