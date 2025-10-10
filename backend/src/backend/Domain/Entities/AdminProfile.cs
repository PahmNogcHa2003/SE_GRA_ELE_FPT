using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class AdminProfile
    {
        public long UserId { get; set; }               
        public string? FullName { get; set; }          
        public string? Position { get; set; }           
        public string? AvatarUrl { get; set; }         
        public DateTimeOffset CreatedAt { get; set; }   
        public DateTimeOffset UpdatedAt { get; set; }   

        public virtual AspNetUser User { get; set; } = null!;
    }
}
