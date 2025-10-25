using AutoMapper;
using AutoMapper.QueryableExtensions;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Domain.Enums;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Visits.Queries;

public record ListVisits(DateTime? From, DateTime? To, VisitStatus? Status, int? InspectorId, string? Category) : IRequest<List<InspectionVisitDto>>;

public class ListVisitsHandler : IRequestHandler<ListVisits, List<InspectionVisitDto>>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public ListVisitsHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<List<InspectionVisitDto>> Handle(ListVisits req, CancellationToken ct)
    {
        var q = _db.InspectionVisits.AsNoTracking().Include(x => x.Violations).AsQueryable();

        if (req.From != null) q = q.Where(x => x.ScheduledAt >= req.From.Value);
        if (req.To != null) q = q.Where(x => x.ScheduledAt < req.To.Value);
        if (req.Status != null) q = q.Where(x => x.Status == req.Status.Value);
        if (req.InspectorId != null) q = q.Where(x => x.InspectorId == req.InspectorId.Value);
        if (!string.IsNullOrWhiteSpace(req.Category))
        {
            q = q.Join(_db.EntitiesToInspect, v => v.EntityToInspectId, e => e.Id, (v,e)=>new {v,e})
                 .Where(joined => joined.e.Category == req.Category)
                 .Select(joined => joined.v);
        }

        return await q.ProjectTo<InspectionVisitDto>(_mapper.ConfigurationProvider).ToListAsync(ct);
    }
}
