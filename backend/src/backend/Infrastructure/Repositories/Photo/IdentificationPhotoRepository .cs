using Application.Interfaces.Photo;
using Application.Photo;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Setting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Photo
{
    public class IdentificationPhotoRepository : IIdentificationPhotoRepository
    {
        private readonly Cloudinary _cloudinary;

        public IdentificationPhotoRepository(IOptions<CloudinarySettings> config)
        {
            var account = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(account);
        }

        /// <summary>
        /// Upload ảnh CCCD lên Cloudinary private
        /// </summary>
        public async Task<PhotoUploadResult> AddPhotoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không được để trống");

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "kyc/cccd",          // Folder riêng cho CCCD
                Type = "private",              // Private CDN
                UseFilename = true,
                UniqueFilename = true,
                Overwrite = false,
                Transformation = new Transformation()
                    .Height(500)
                    .Width(500)
                    .Crop("fill")             // Resize ảnh
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = null // Không trả URL công khai, chỉ trả PublicId
            };
        }

       
        public async Task<string> DeletePhotoAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("PublicId không hợp lệ");

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }


        public string GetSignedUrl(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                throw new ArgumentException("PublicId không hợp lệ");

            // Tạo URL private với signed = true
            var url = _cloudinary.Api.UrlImgUp
                .PrivateCdn(true)
                .Signed(true)
                .BuildUrl(publicId);

            return url;
        }

    }
}
