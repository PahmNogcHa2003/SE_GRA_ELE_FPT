using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Promotion;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Repository;
using Application.Interfaces.Staff.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;

namespace Application.Services.Staff
{
    public class PromotionCampaignService : GenericService<PromotionCampaign, PromotionDTO , long>, IPromotionCampaignService
    {
        private readonly IPromotionCampaignRepository _promotionCampaignRepository;
        public PromotionCampaignService(
            IRepository<PromotionCampaign, long> repo,
            IMapper mapper,
            IUnitOfWork uow,
            IPromotionCampaignRepository promotionCampaignRepository
        ) : base(repo, mapper, uow)
        {
            _promotionCampaignRepository = promotionCampaignRepository;
        }
        public async Task<PromotionDTO> CreateAsync(PromotionCreateDTO dto , CancellationToken ct)
        {
            var entity = _mapper.Map<PromotionCampaign>(dto);
            entity.Status = "Active";
            await _promotionCampaignRepository.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<PromotionDTO>(entity);
        }
        public async Task<PromotionDTO> UpdateStatusAsync(long id, string status, CancellationToken ct)
        {
            var promo = await _promotionCampaignRepository.GetByIdAsync(id, ct);
            if(promo == null)
            {
                throw new KeyNotFoundException($"Promotion with ID {id} not found.");
            }
            promo.Status = status;
            _promotionCampaignRepository.Update(promo);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<PromotionDTO>(promo);
        }
        protected override IQueryable<PromotionCampaign> ApplySearch(IQueryable<PromotionCampaign> query, string searchQuery)
        {
            return query.Where(p => p.Name.Contains(searchQuery) || p.Description!.Contains(searchQuery));
        }
    }
}
