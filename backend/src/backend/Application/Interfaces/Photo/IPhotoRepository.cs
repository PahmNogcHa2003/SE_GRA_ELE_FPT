using Application.Photo;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Photo
{
    public interface IPhotoRepository
    {
        Task<PhotoUploadResult> AddPhotoAsync(IFormFile file);
        Task<string> DeletePhotoAsync(string publicId);
    }
}
