using Application.DTOs.Kyc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Ocr
{
    public interface IOcrRepository
    {
        Task<KycDTO> ReadIdCardInfoAsync(string frontImageUrl, string backImageUrl);
    }
}
