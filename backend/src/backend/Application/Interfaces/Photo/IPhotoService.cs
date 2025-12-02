using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Photo;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Photo
{
    public interface IPhotoService
    {
        Task<PhotoUploadResult> AddPhotoAsync(IFormFile file, PhotoPreset preset = PhotoPreset.Default);
        Task<string> DeletePhotoAsync(string publicId);
    }
}
