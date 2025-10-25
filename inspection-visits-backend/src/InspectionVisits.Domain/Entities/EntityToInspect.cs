using InspectionVisits.Domain.Common;

namespace InspectionVisits.Domain.Entities;
public class EntityToInspect : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Address { get; set; } = default!;
    public string Category { get; set; } = default!;
}
