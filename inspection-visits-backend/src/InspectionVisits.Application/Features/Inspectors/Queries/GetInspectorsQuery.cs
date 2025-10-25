using AutoMapper;
using AutoMapper.QueryableExtensions;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Inspectors.Queries;


public sealed record GetInspectorsQuery : IRequest<IReadOnlyList<InspectorDto>>;

public sealed class Handler : IRequestHandler<GetInspectorsQuery, IReadOnlyList<InspectorDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public Handler(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<InspectorDto>> Handle(GetInspectorsQuery request, CancellationToken ct)
    {
        return await _db.Inspectors
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .ProjectTo<InspectorDto>(_mapper.ConfigurationProvider)
            .ToListAsync(ct);
    }
}

