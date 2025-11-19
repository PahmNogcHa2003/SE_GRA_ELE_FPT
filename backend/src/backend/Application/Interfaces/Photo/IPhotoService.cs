using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Photo;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Photo
{
    public interface IPhotoService
    {
        Task<PhotoUploadResult> AddPhotoAsync(IFormFile file);
        Task<string> DeletePhotoAsync(string publicId);
    }
}
