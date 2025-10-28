using Application.DTOs.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Email
{
    public interface IEmailService
    {
        Task<bool> SendAsync(MailData Mail_Data); 

    }
}
