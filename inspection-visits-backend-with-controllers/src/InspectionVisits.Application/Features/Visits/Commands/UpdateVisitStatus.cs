using FluentValidation;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Domain.Enums;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace InspectionVisits.Application.Features.Visits.Commands;

public record UpdateVisitStatus(int Id, VisitStatus NewStatus, int? Score, string? Notes) : IRequest<InspectionVisitDto>;

public class UpdateVisitStatusValidator : AbstractValidator<UpdateVisitStatus>
{
    public UpdateVisitStatusValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.NewStatus).IsInEnum();
        When(x => x.NewStatus == VisitStatus.Completed, () =>
        {
            RuleFor(x => x.Score).NotNull().InclusiveBetween(0, 100);
        });
    }
}

public class UpdateVisitStatusHandler : IRequestHandler<UpdateVisitStatus, InspectionVisitDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public UpdateVisitStatusHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<InspectionVisitDto> Handle(UpdateVisitStatus req, CancellationToken ct)
    {
        var v = await _db.InspectionVisits.Include(x => x.Violations).FirstOrDefaultAsync(x => x.Id == req.Id, ct)
                ?? throw new KeyNotFoundException("VISIT_NOT_FOUND");

        // workflow: Planned → InProgress → Completed/Cancelled
        if (v.Status == VisitStatus.Planned && req.NewStatus == VisitStatus.InProgress) v.Status = VisitStatus.InProgress;
        else if (v.Status == VisitStatus.InProgress && (req.NewStatus == VisitStatus.Completed || req.NewStatus == VisitStatus.Cancelled))
        {
            v.Status = req.NewStatus;
            if (req.NewStatus == VisitStatus.Completed)
            {
                v.Score = req.Score;
                v.Notes = req.Notes;
            }
        }
        else
        {
            throw new InvalidOperationException("INVALID_STATUS_TRANSITION");
        }

        v.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<InspectionVisitDto>(v);
    }
}
