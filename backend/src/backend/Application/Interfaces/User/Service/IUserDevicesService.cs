using Application.DTOs.UserDevice;
using Application.Interfaces.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IUserDevicesService : IService<Domain.Entities.UserDevice, DTOs.UserDevice.UserDeviceDTO, long>
    {
        Task HandleDeviceLoginAsync(CreateUserDeviceDTO dto, CancellationToken ct = default);
    }
}
