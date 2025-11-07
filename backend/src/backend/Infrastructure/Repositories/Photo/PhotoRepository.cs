using Application.Interfaces.Photo;
using Application.Photo;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Photo
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly Cloudinary _cloudinary;
        public PhotoRepository(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile file)
        {
            if (file.Length > 0)
            {
                // Mở stream để đọc file
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    // (Tùy chọn) Biến đổi ảnh, ví dụ: cắt và resize
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill")
                };

                // Gọi API của Cloudinary để upload
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new System.Exception(uploadResult.Error.Message);
                }

                return new PhotoUploadResult
                {
                    PublicId = uploadResult.PublicId,
                    Url = uploadResult.SecureUrl.ToString()
                };
            }

            return null;
        }

        public async Task<string> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}
