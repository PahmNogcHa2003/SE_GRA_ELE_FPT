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
    public interface IContactService : IService<Contact, CreateContactDTO, long>,
          IService3DTO<Contact, CreateContactDTO , long>
    {
    }
}
