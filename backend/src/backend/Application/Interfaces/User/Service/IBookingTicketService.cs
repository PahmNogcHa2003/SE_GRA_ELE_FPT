using Application.DTOs.BookingTicket;
using Application.DTOs.Tickets;
using Application.Interfaces.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.User.Service
{
    public interface IBookingTicketService : IService<BookingTicket, CreateBookingTicketDTO, long>, IService3DTO<BookingTicket, CreateBookingTicketDTO, long>
    {
    }
}
