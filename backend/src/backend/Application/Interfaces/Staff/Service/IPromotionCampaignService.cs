using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Promotion;
using Application.Interfaces.Base;

namespace Application.Interfaces.Staff.Service
{
    public interface IPromotionCampaignService : IService<Domain.Entities.PromotionCampaign, PromotionDTO, long>, IService3DTO<Domain.Entities.PromotionCampaign, PromotionDTO, long>
    {
        Task<PromotionDTO> CreateAsync(PromotionCreateDTO dto, CancellationToken ct);
        Task<PromotionDTO> UpdateStatusAsync(long id, string status, CancellationToken ct);
    }
}
