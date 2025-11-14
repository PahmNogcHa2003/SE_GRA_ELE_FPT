using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Photo;
using Application.Photo;

namespace Application.Services.Photo
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;
        public PhotoService(IPhotoRepository photoRepository) 
        {
            _photoRepository = photoRepository;
        }
        public async Task<PhotoUploadResult> AddPhotoAsync(Microsoft.AspNetCore.Http.IFormFile file)
        {
            return await _photoRepository.AddPhotoAsync(file);
        }
        public async Task<string> DeletePhotoAsync(string publicId)
        {
            return await _photoRepository.DeletePhotoAsync(publicId);
        }
    }
}
