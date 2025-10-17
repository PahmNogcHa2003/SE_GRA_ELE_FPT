using Application.DTOs.Location;
using Application.Interfaces;
using Application.Interfaces.Location;
using AutoMapper;
using MediatR;

namespace Application.Services.Location;

public record GetWardsByProvinceQuery(int ProvinceCode) : IRequest<IEnumerable<LocationDTO>>;

public class GetWardsByProvinceQueryHandler : IRequestHandler<GetWardsByProvinceQuery, IEnumerable<LocationDTO>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public GetWardsByProvinceQueryHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LocationDTO>> Handle(GetWardsByProvinceQuery request, CancellationToken cancellationToken)
    {
        var wards = await _locationRepository.GetWardsByProvinceAsync(request.ProvinceCode);
        return _mapper.Map<IEnumerable<LocationDTO>>(wards);
    }
}