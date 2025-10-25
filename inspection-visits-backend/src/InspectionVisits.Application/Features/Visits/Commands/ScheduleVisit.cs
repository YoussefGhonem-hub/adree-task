using FluentValidation;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Domain.Entities;
using InspectionVisits.Domain.Enums;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Visits.Commands;

public record ScheduleVisit(int EntityToInspectId, int InspectorId, DateTime ScheduledAt) : IRequest<InspectionVisitDto>;

public class ScheduleVisitValidator : AbstractValidator<ScheduleVisit>
{
    public ScheduleVisitValidator()
    {
        RuleFor(x => x.EntityToInspectId).GreaterThan(0);
        RuleFor(x => x.InspectorId).GreaterThan(0);
        RuleFor(x => x.ScheduledAt).GreaterThan(DateTime.UtcNow.AddMinutes(-1));
    }
}

public class ScheduleVisitHandler : IRequestHandler<ScheduleVisit, InspectionVisitDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public ScheduleVisitHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<InspectionVisitDto> Handle(ScheduleVisit req, CancellationToken ct)
    {
        // ensure refs exist
        if (!await _db.EntitiesToInspect.AnyAsync(e => e.Id == req.EntityToInspectId, ct))
            throw new KeyNotFoundException("ENTITY_NOT_FOUND");

        if (!await _db.Inspectors.AnyAsync(i => i.Id == req.InspectorId, ct))
            throw new KeyNotFoundException("INSPECTOR_NOT_FOUND");

        var visit = new InspectionVisit
        {
            EntityToInspectId = req.EntityToInspectId,
            InspectorId = req.InspectorId,
            ScheduledAt = req.ScheduledAt.ToUniversalTime(),
            Status = VisitStatus.Planned
        };
        _db.InspectionVisits.Add(visit);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<InspectionVisitDto>(visit);
    }
}
