using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public static class UserTicketStatus
    {
        public const string Ready = "Ready";
        public const string Active = "Active";
        public const string Used = "Used";
        public const string Expired = "Expired";
    }
}
