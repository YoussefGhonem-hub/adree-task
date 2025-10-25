using AutoMapper;
using FluentValidation;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Domain.Entities;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;

namespace InspectionVisits.Application.Features.Entities.Commands;

public record CreateEntityToInspect(string Name, string Address, string Category) : IRequest<EntityToInspectDto>;

public class CreateEntityToInspectValidator : AbstractValidator<CreateEntityToInspect>
{
    public CreateEntityToInspectValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(400);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}

public class CreateEntityToInspectHandler : IRequestHandler<CreateEntityToInspect, EntityToInspectDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public CreateEntityToInspectHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<EntityToInspectDto> Handle(CreateEntityToInspect req, CancellationToken ct)
    {
        var entity = new EntityToInspect { Name = req.Name, Address = req.Address, Category = req.Category };
        _db.EntitiesToInspect.Add(entity);
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<EntityToInspectDto>(entity);
    }
}
