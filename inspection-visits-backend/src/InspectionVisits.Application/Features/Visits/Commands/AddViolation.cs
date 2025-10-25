using FluentValidation;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Domain.Entities;
using InspectionVisits.Domain.Enums;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace InspectionVisits.Application.Features.Visits.Commands;

public record AddViolation(int VisitId, string Code, string Description, ViolationSeverity Severity) : IRequest<ViolationDto>;

public class AddViolationValidator : AbstractValidator<AddViolation>
{
    public AddViolationValidator()
    {
        RuleFor(x => x.VisitId).GreaterThan(0);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Severity).IsInEnum();
    }
}

public class AddViolationHandler : IRequestHandler<AddViolation, ViolationDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public AddViolationHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<ViolationDto> Handle(AddViolation req, CancellationToken ct)
    {
        var visit = await _db.InspectionVisits.FirstOrDefaultAsync(x => x.Id == req.VisitId, ct)
                    ?? throw new KeyNotFoundException("VISIT_NOT_FOUND");

        if (visit.Status != Domain.Enums.VisitStatus.Completed)
            throw new InvalidOperationException("VIOLATIONS_ALLOWED_ONLY_AFTER_COMPLETION");

        var v = new Violation { InspectionVisitId = req.VisitId, Code = req.Code, Description = req.Description, Severity = req.Severity };
        _db.Violations.Add(v);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<ViolationDto>(v);
    }
}
