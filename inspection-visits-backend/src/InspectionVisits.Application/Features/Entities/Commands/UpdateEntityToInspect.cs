using AutoMapper;
using FluentValidation;
using InspectionVisits.Application.DTOs;
using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Entities.Commands;

public record UpdateEntityToInspect(int Id, string Name, string Address, string Category) : IRequest<EntityToInspectDto>;

public class UpdateEntityToInspectValidator : AbstractValidator<UpdateEntityToInspect>
{
    public UpdateEntityToInspectValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(400);
        RuleFor(x => x.Category).NotEmpty().MaximumLength(100);
    }
}

public class UpdateEntityToInspectHandler : IRequestHandler<UpdateEntityToInspect, EntityToInspectDto>
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;
    public UpdateEntityToInspectHandler(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

    public async Task<EntityToInspectDto> Handle(UpdateEntityToInspect req, CancellationToken ct)
    {
        var entity = await _db.EntitiesToInspect.FirstOrDefaultAsync(x => x.Id == req.Id, ct) 
                     ?? throw new KeyNotFoundException("ENTITY_NOT_FOUND");

        entity.Name = req.Name; entity.Address = req.Address; entity.Category = req.Category;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return _mapper.Map<EntityToInspectDto>(entity);
    }
}
