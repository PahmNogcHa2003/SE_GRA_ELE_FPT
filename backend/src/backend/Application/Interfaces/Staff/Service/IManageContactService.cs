using Application.DTOs.Contact;
using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Staff.Service
{
    public interface IManageContactService : IService<Contact, ManageContactDTO , long>,
          IService3DTO<Contact, ManageContactDTO, long>
    {
    }
}
