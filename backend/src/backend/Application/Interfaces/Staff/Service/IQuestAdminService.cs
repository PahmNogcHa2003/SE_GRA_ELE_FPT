using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Quest;
using Application.Interfaces.Base;
using Domain.Entities;

namespace Application.Interfaces.Staff.Service
{
    public interface IQuestAdminService : IService<Quest, QuestDTO, long> , IService3DTO<Quest, QuestDTO, long>
    {
        Task<QuestDTO> CreateAsync(QuestCreateDTO dto, CancellationToken ct = default);
        Task UpdateAsync(long id, QuestUpdateDTO dto, CancellationToken ct = default);
        Task ToggleStatusAsync(long id, CancellationToken ct = default);

    }
}
