using Application.DTOs.UserDevice;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using Microsoft.EntityFrameworkCore; 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class UserDevicesService : GenericService<UserDevice, UserDeviceDTO, long>, IUserDevicesService
    {
        public UserDevicesService(IRepository<UserDevice, long> repo, IMapper mapper, IUnitOfWork uow)
            : base(repo, mapper, uow)
        {
        }

        public async Task HandleDeviceLoginAsync(CreateUserDeviceDTO dto, CancellationToken ct = default)
        {
            // Giờ đây, FirstOrDefaultAsync đã được nhận diện
            var device = await _repo.Query()
                .FirstOrDefaultAsync(d => d.UserId == dto.UserId && d.DeviceId.Equals(dto.DeviceId), ct);

            var now = DateTimeOffset.UtcNow;

            if (device == null)
            {
                // Nếu thiết bị chưa tồn tại, tạo mới
                var newDevice = new UserDevice
                {
                    UserId = dto.UserId,
                    DeviceId = dto.DeviceId,
                    Platform = dto.Platform,
                    PushToken = dto.PushToken,
                    LastLoginAt = now,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _repo.AddAsync(newDevice, ct);
            }
            else
            {
                // Nếu đã tồn tại, cập nhật PushToken và thời gian đăng nhập
                device.PushToken = dto.PushToken;
                device.LastLoginAt = now;
                device.UpdatedAt = now;
                _repo.Update(device);
            }

            // Lưu các thay đổi vào cơ sở dữ liệu
            await _uow.SaveChangesAsync(ct);
        }
    }
}

