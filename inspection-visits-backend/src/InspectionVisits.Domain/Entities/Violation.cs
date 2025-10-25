using InspectionVisits.Domain.Common;
using InspectionVisits.Domain.Enums;

namespace InspectionVisits.Domain.Entities;
public class Violation : BaseEntity
{
    public int InspectionVisitId { get; set; }
    public string Code { get; set; } = default!;
    public string Description { get; set; } = default!;
    public ViolationSeverity Severity { get; set; }

    public InspectionVisit? InspectionVisit { get; set; }
}
