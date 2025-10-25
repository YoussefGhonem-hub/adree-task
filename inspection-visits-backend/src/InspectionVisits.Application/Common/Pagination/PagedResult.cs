namespace InspectionVisits.Application.Common.Pagination;
public sealed class PagedResult<T>
{
    public int PageIndex { get; init; }
    public int PageSize { get; init; }
    public int Total { get; init; }
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
}
