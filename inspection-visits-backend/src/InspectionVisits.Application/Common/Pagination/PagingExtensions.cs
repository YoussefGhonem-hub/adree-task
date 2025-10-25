namespace InspectionVisits.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

public static class PagingExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageIndex, int pageSize, CancellationToken ct = default)
    {
        var total = await query.CountAsync(ct);
        var items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<T> { PageIndex = pageIndex, PageSize = pageSize, Total = total, Items = items };
    }
}
