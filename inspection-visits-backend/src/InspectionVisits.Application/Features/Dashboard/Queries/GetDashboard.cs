using MediatR;
using InspectionVisits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Application.Features.Dashboard.Queries;

public record DashboardVm(Dictionary<string,int> CountsByStatus, double AverageScoreThisMonth);
public record GetDashboard() : IRequest<DashboardVm>;

public class GetDashboardHandler : IRequestHandler<GetDashboard, DashboardVm>
{
    private readonly AppDbContext _db;
    public GetDashboardHandler(AppDbContext db) { _db = db; }

    public async Task<DashboardVm> Handle(GetDashboard request, CancellationToken ct)
    {
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var counts = await _db.InspectionVisits
            .GroupBy(v => v.Status)
            .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, ct);

        var avg = await _db.InspectionVisits
            .Where(v => v.ScheduledAt >= monthStart && v.Score != null)
            .AverageAsync(v => (double?)v.Score!, ct) ?? 0.0;

        return new DashboardVm(counts, Math.Round(avg, 2));
    }
}
