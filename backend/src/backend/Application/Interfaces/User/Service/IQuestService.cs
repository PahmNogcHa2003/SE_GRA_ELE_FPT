using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Quest;

namespace Application.Interfaces.User.Service
{
    public interface IQuestService
    {
        Task ProcessRideAsync(long userId,decimal distanceKm, int durationMinutes, DateTimeOffset rideTimeUtc,CancellationToken ct);
        Task<IReadOnlyList<QuestDTO>> GetMyActiveQuestsAsync(long? userId, CancellationToken ct);
    }
}
