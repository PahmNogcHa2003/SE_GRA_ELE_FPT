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
    public interface IReplyContactService 
    {
        Task<bool> ReplyToContactAsync(long contactId, ReplyContactDTO dto, long staffId, CancellationToken cancellationToken = default);
    }
}
