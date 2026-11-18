using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbAccessaLayer.Helpers
{
    public static class ImageHelper
    {
        public static async Task<string> SaveProductIamges(IFormFile formFile, IWebHostEnvironment hostEnvironment)
        {
            if(formFile==null||formFile.Length==0) throw new ArgumentNullException("Invalid image file");

            //combine folder path like C:\...\wwwroot\uploads\products
            var folder = Path.Combine(hostEnvironment.WebRootPath, "uploads","products");

            if(!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            //it generates unique id for each file like b7c2e7f4-2c8a-4f34-9b12-01a8.jpg
            var fileName =$"{Guid.NewGuid()}{Path.GetExtension(formFile.FileName)}";

            //now final path becomes C:\...\wwwroot\uploads\products\b7c2e7f4.jpg
            var filepath =Path.Combine(folder, fileName);

            //saving and write into the file 
            using var stream = new FileStream(filepath, FileMode.Create);
            await formFile.CopyToAsync(stream);
            return $"/uploads/products/{fileName}";
        }


        public static async Task<string> UpdateProductImages(IFormFile file, IWebHostEnvironment webHostEnvironment, string? oldImagePath=null)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception($"Inavlid image File");
            }

            string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads", "products");

            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            //delete old images

            if (!string.IsNullOrEmpty(oldImagePath))
            {
                string oldPath = Path.Combine(webHostEnvironment.WebRootPath, oldImagePath.TrimStart('/'));
                if (File.Exists(oldPath))
                {
                    File.Delete(oldPath);
                }
            }

            string fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string fullPath = Path.Combine(uploadFolder, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);
            return $"/uploads/products/{fileName}";
        }


        public static async Task<string> SaveCategoryImage(IFormFile file, IWebHostEnvironment env)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid image");

            var folder = Path.Combine(env.WebRootPath, "uploads", "categories");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/categories/{fileName}";
        }
    }
}
