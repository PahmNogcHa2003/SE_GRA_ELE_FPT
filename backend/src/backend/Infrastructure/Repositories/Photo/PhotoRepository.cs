using Application.Interfaces.Photo;
using Application.Photo;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Enums;
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

        public async Task<PhotoUploadResult?> AddPhotoAsync(IFormFile file, PhotoPreset preset = PhotoPreset.Default)
        {
            if (file.Length <= 0) return null;

            await using var stream = file.OpenReadStream();

            var transformation = preset switch
            {
                PhotoPreset.Avatar => new Transformation()
                    .Width(400).Height(400).Crop("fill").Gravity("face"),

                PhotoPreset.Station => new Transformation()
                    .AspectRatio("4:3").Width(1200).Crop("fill"),

                PhotoPreset.NewsBanner => new Transformation()
                    .AspectRatio("16:9").Width(1600).Crop("fill"),

                _ => new Transformation()
                    .Quality("auto").FetchFormat("auto")
            };

            var folder = preset switch
            {
                PhotoPreset.Avatar => "holabike/avatars",
                PhotoPreset.Station => "holabike/stations",
                PhotoPreset.NewsBanner => "holabike/news",
                _ => "holabike/others"
            };

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = transformation,
                Folder = folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception(uploadResult.Error.Message);

            return new PhotoUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString()
            };
        }


        public async Task<string> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok" ? result.Result : null;
        }
    }
}
