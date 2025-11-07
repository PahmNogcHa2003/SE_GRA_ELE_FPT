using Application.DTOs.Kyc;
using Application.DTOs.Rental;
using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IKycService 
    {
        Task<bool> CreateKycAsync(CreateKycRequestDTO createKycRequestDTO);
    }
}
