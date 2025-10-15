using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class AppMappingProfile : Profile
    {
        public AppMappingProfile()
        {
            // Example mapping configuration
            // CreateMap<Source, Destination>();
            CreateMap<Domain.Entities.AspNetUser, DTOs.UserDTO>().ReverseMap();
            CreateMap<Domain.Entities.Station, DTOs.StationDTO>().ReverseMap();
        }
    }
}
