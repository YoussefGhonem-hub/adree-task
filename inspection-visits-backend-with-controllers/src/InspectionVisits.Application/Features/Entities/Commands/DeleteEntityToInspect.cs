using InspectionVisits.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Entities.Commands;

public record DeleteEntityToInspect(int Id) : IRequest<Unit>;

public class DeleteEntityToInspectHandler : IRequestHandler<DeleteEntityToInspect, Unit>
{
    private readonly AppDbContext _db;
    public DeleteEntityToInspectHandler(AppDbContext db) { _db = db; }

    public async Task<Unit> Handle(DeleteEntityToInspect req, CancellationToken ct)
    {
        var entity = await _db.EntitiesToInspect.FirstOrDefaultAsync(x => x.Id == req.Id, ct)
                     ?? throw new KeyNotFoundException("ENTITY_NOT_FOUND");
        _db.EntitiesToInspect.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
