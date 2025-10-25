using AutoMapper;
using AutoMapper.QueryableExtensions;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Entities.Queries;

public record ListEntitiesToInspect(string? Category) : IRequest<List<EntityToInspectDto>>;

public class ListEntitiesToInspectHandler : IRequestHandler<ListEntitiesToInspect, List<EntityToInspectDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public ListEntitiesToInspectHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<List<EntityToInspectDto>> Handle(ListEntitiesToInspect req, CancellationToken ct)
    {
        var q = _db.EntitiesToInspect.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Category))
            q = q.Where(x => x.Category == req.Category);
        return await q.ProjectTo<EntityToInspectDto>(_mapper.ConfigurationProvider).ToListAsync(ct);
    }
}
