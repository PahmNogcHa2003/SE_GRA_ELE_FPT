using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Quest;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Repository;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Staff
{
    public class QuestAdminService : GenericService<Quest, QuestDTO, long>, IQuestAdminService
    {
        private readonly IUserQuestProgressRepository _progressRepo;
        public QuestAdminService(
            IRepository<Quest, long> repo,
            IMapper mapper,
            IUserQuestProgressRepository progressRepo,
            IUnitOfWork uow
        ) : base(repo, mapper, uow)
        {
            _progressRepo = progressRepo;
        }
        private static void ValidateQuest(QuestCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Code không được để trống.");

            if (!QuestTypes.All.Contains(dto.QuestType))
                throw new ArgumentException($"QuestType không hợp lệ. Hợp lệ: {string.Join(", ", QuestTypes.All)}");

            if (!QuestScopes.All.Contains(dto.Scope))
                throw new ArgumentException($"Scope không hợp lệ. Hợp lệ: {string.Join(", ", QuestScopes.All)}");

            if (dto.StartAt >= dto.EndAt)
                throw new ArgumentException("StartAt phải nhỏ hơn EndAt.");

            var typeLower = dto.QuestType.Trim().ToLowerInvariant();
            switch (typeLower)
            {
                case "distance":
                    if (!dto.TargetDistanceKm.HasValue || dto.TargetDistanceKm.Value <= 0)
                        throw new ArgumentException("Quest Distance phải có TargetDistanceKm > 0.");
                    break;
                case "trips":
                    if (!dto.TargetTrips.HasValue || dto.TargetTrips.Value <= 0)
                        throw new ArgumentException("Quest Trips phải có TargetTrips > 0.");
                    break;
                case "duration":
                    if (!dto.TargetDurationMinutes.HasValue || dto.TargetDurationMinutes.Value <= 0)
                        throw new ArgumentException("Quest Duration phải có TargetDurationMinutes > 0.");
                    break;
            }

            if (dto.PromoReward < 0)
                throw new ArgumentException("PromoReward không được âm.");
        }

        public async Task<QuestDTO> CreateAsync (QuestCreateDTO dto, CancellationToken ct = default)
        {
            ValidateQuest(dto);
            var exists = await _repo.Query().AnyAsync(q => q.Code == dto.Code, ct);
            if (exists)
                throw new ArgumentException($"Quest với Code '{dto.Code}' đã tồn tại.");
            var entity = _mapper.Map<Quest>(dto);
            entity.Status = QuestStatus.Inactive;
            entity.UpdatedAt = DateTimeOffset.Now;
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return _mapper.Map<QuestDTO>(entity);
        }
        public async Task UpdateAsync(long id, QuestUpdateDTO dto, CancellationToken ct = default)
        {
            ValidateQuest(dto);

            var entity = await _repo.Query()
                .FirstOrDefaultAsync(q => q.Id == id, ct)
                ?? throw new KeyNotFoundException($"Quest id={id} không tồn tại.");

            var exists = await _repo.Query()
                .AnyAsync(q => q.Id != id && q.Code == dto.Code, ct);
            if (exists)
                throw new InvalidOperationException($"Quest với Code '{dto.Code}' đã tồn tại cho quest khác.");

            var currentStatus = entity.Status;

            _mapper.Map(dto, entity);
            entity.Status = currentStatus;           
            entity.UpdatedAt = DateTimeOffset.UtcNow;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
        }


        protected override IQueryable<Quest> ApplySearch(IQueryable<Quest> query, string searchQuery)
        {
            searchQuery = searchQuery.Trim();
            return query.Where(q =>
                q.Code.Contains(searchQuery) ||
                q.Title.Contains(searchQuery) ||
                (q.Description != null && q.Description.Contains(searchQuery)));
        }
        public async Task ToggleStatusAsync(long id, CancellationToken ct = default)
        {
            var quest = await _repo.Query()
                .FirstOrDefaultAsync(q => q.Id == id, ct)
                ?? throw new KeyNotFoundException($"Quest id={id} không tồn tại.");

            var hasProgress = await _progressRepo.AnyProgressForQuestAsync(id, ct);

            if (quest.Status == QuestStatus.Active)
            {
                if (hasProgress)
                    throw new InvalidOperationException("Không thể tắt Quest đã có người dùng tham gia hoặc hoàn thành.");

                quest.Status = QuestStatus.Inactive;
            }
            else
            {
                quest.Status = QuestStatus.Active;
            }
            quest.UpdatedAt = DateTimeOffset.UtcNow; 
            _repo.Update(quest);
            await _uow.SaveChangesAsync(ct);
        }

    }
}
