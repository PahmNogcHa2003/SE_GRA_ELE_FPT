using Application.DTOs.Location;
using Application.Interfaces;
using Application.Interfaces.Location;
using AutoMapper;
using MediatR;

namespace Application.Services.Location;

public record GetProvincesQuery : IRequest<IEnumerable<LocationDTO>>;
public class GetProvincesQueryHandler : IRequestHandler<GetProvincesQuery, IEnumerable<LocationDTO>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IMapper _mapper;

    public GetProvincesQueryHandler(ILocationRepository locationRepository, IMapper mapper)
    {
        _locationRepository = locationRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LocationDTO>> Handle(GetProvincesQuery request, CancellationToken cancellationToken)
    {
        var provinces = await _locationRepository.GetProvincesAsync();
        return _mapper.Map<IEnumerable<LocationDTO>>(provinces);
    }
}