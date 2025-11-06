using Application.DTOs;
using Application.DTOs.Contact;
using Application.Interfaces;
using Application.Interfaces.Base;
using Application.Interfaces.Staff.Service;
using Application.Interfaces.User.Service;
using Application.Services.Base;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.User
{
    public class ContactService : GenericService<Contact, CreateContactDTO, long>, IContactService
    {
        public ContactService(IRepository<Contact, long> repo, IMapper mapper, IUnitOfWork uow) : base(repo, mapper, uow)
        {
        }
    }
}
