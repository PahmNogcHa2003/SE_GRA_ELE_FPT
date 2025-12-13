using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Base;
using Application.Interfaces;
using Application.Interfaces.User.Service;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Application.DTOs.Quest;
using System.Diagnostics;

namespace Application.Services.User
{
    public class QuestService : IQuestService
    {
        private readonly IRepository<Quest, long> _questRepo;
        private readonly IRepository<UserQuestProgress, long> _progressRepo;
        private readonly IWalletService _walletService;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        public QuestService(
            IRepository<Quest, long> questRepo,
            IRepository<UserQuestProgress, long> progressRepo,
            IWalletService walletService,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _questRepo = questRepo;
            _progressRepo = progressRepo;
            _walletService = walletService;
            _uow = uow;
            _mapper = mapper;
        }
        public async Task ProcessRideAsync(long userId, decimal distanceKm, int durationMinutes, DateTimeOffset rideTimeUtc, CancellationToken ct)
        {
            if (userId < 0)
                throw new ArgumentException("Invalid userId");
            if (distanceKm < 0)
                throw new ArgumentException("Invalid distanceKm");
            if (durationMinutes < 0)
                throw new ArgumentException("Invalid durationMinutes");

            var activeQuests = await _questRepo.Query()
                .Where(q =>
                    q.Status == "Active" &&
                    q.StartAt <= rideTimeUtc &&
                    q.EndAt >= rideTimeUtc
                ).ToListAsync(ct);

            if (!activeQuests.Any())
                return;

            foreach (var quest in activeQuests)
            {
                var progress = await _progressRepo.Query()
                    .FirstOrDefaultAsync(p => p.UserId == userId && p.QuestId == quest.Id, ct);

                bool isNew = false;

                if (progress == null)
                {
                    progress = new UserQuestProgress
                    {
                        UserId = userId,
                        QuestId = quest.Id,
                        CurrentDistanceKm = 0,
                        CurrentTrips = 0,
                        CurrentDurationMinutes = 0,
                        IsCompleted = false,
                        LastUpdatedAt = DateTimeOffset.UtcNow
                    };

                    await _progressRepo.AddAsync(progress, ct);
                    isNew = true;
                }

                var questType = quest.QuestType?.Trim().ToLowerInvariant() ?? "distance";

                switch (questType)
                {
                    case "distance":
                        if (quest.TargetDistanceKm > 0)
                            progress.CurrentDistanceKm += distanceKm;
                        break;

                    case "trips":
                        if (quest.TargetTrips > 0)
                            progress.CurrentTrips += 1;
                        break;

                    case "duration":
                        if (quest.TargetDurationMinutes > 0)
                            progress.CurrentDurationMinutes += durationMinutes;
                        break;
                }

                progress.LastUpdatedAt = DateTimeOffset.UtcNow;

                // Tính %
                decimal progressPercent = questType switch
                {
                    "distance" => quest.TargetDistanceKm > 0
                        ? Math.Min(100, (progress.CurrentDistanceKm / quest.TargetDistanceKm.Value) * 100)
                        : 0,

                    "trips" => quest.TargetTrips > 0
                        ? Math.Min(100, ((decimal)progress.CurrentTrips / quest.TargetTrips.Value) * 100)
                        : 0,

                    "duration" => quest.TargetDurationMinutes > 0
                        ? Math.Min(100, ((decimal)progress.CurrentDurationMinutes / quest.TargetDurationMinutes.Value) * 100)
                        : 0,

                    _ => 0
                };

                // Hoàn thành
                if (!progress.IsCompleted && progressPercent >= 100)
                {
                    progress.IsCompleted = true;
                    progress.CompletedAt = DateTimeOffset.UtcNow;

                    if (progress.RewardClaimedAt == null && quest.PromoReward > 0)
                    {
                        await _walletService.CreditPromoAsync(
                            userId,
                            quest.PromoReward,
                            $"Reward for completing quest {quest.Code}",
                            ct
                        );

                        progress.RewardClaimedAt = DateTimeOffset.UtcNow;
                    }
                }

                if (!isNew)
                    _progressRepo.Update(progress);
            }

            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<QuestDTO>> GetMyActiveQuestsAsync(long? userId, CancellationToken ct)
        {
           if(userId < 0)
                throw new ArgumentException("Invalid userId");
           var now = DateTimeOffset.UtcNow;
           var query = 
                from q in _questRepo.Query()
                where q.Status == "Active" && q.StartAt <= now
                    && q.EndAt >= now
                    join p in _progressRepo.Query().Where(x => x.UserId == userId)
                        on q.Id equals p.QuestId into qp
                from p in qp.DefaultIfEmpty()
                orderby q.EndAt ascending
                select new { Quest = q, Progress = p };

            var list = await query.AsNoTracking().ToListAsync(ct);
            var result = new List<QuestDTO>();

            foreach (var item in list)
            {
                var q = item.Quest;
                var p = item.Progress;

                var dto = new QuestDTO
                {
                    Id = q.Id,
                    Code = q.Code,
                    Title = q.Title,
                    Description = q.Description,
                    QuestType = q.QuestType,
                    Scope = q.Scope,
                    TargetDistanceKm = q.TargetDistanceKm,
                    TargetTrips = q.TargetTrips,
                    TargetDurationMinutes = q.TargetDurationMinutes,
                    PromoReward = q.PromoReward,
                    StartAt = q.StartAt,
                    EndAt = q.EndAt,
                    Status = q.Status,

                    CurrentDistanceKm = p?.CurrentDistanceKm ?? 0,
                    CurrentTrips = p?.CurrentTrips ?? 0,
                    CurrentDurationMinutes = p?.CurrentDurationMinutes ?? 0,
                    IsCompleted = p?.IsCompleted ?? false,
                    CompletedAt = p?.CompletedAt,
                    RewardClaimedAt = p?.RewardClaimedAt,
                };
                dto.ProgressPercent = CalculateProgressPercent(q, dto);
                result.Add(dto);
            }
            return result;
        }
        private static decimal CalculateProgressPercent(Quest q, QuestDTO dto)
        {
            var questType = q.QuestType?.Trim().ToLowerInvariant() ?? "distance";
            decimal percent = 0m;

            switch (questType)
            {
                case "distance":
                    if (q.TargetDistanceKm.HasValue && q.TargetDistanceKm.Value > 0)
                        percent = Math.Min(100, (dto.CurrentDistanceKm / q.TargetDistanceKm.Value) * 100);
                    break;

                case "trips":
                    if (q.TargetTrips.HasValue && q.TargetTrips.Value > 0)
                        percent = Math.Min(100, ((decimal)dto.CurrentTrips / q.TargetTrips.Value) * 100);
                    break;

                case "duration":
                case "time":
                    if (q.TargetDurationMinutes.HasValue && q.TargetDurationMinutes.Value > 0)
                        percent = Math.Min(100, ((decimal)dto.CurrentDurationMinutes / q.TargetDurationMinutes.Value) * 100);
                    break;
            }

            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;
            return percent;
        }
    }
}
