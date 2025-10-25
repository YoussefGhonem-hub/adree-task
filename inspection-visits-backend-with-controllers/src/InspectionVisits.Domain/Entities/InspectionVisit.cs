using InspectionVisits.Domain.Common;
using InspectionVisits.Domain.Enums;

namespace InspectionVisits.Domain.Entities;
public class InspectionVisit : BaseEntity
{
    public int EntityToInspectId { get; set; }
    public int InspectorId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Planned;
    public int? Score { get; set; }
    public string? Notes { get; set; }

    public EntityToInspect? EntityToInspect { get; set; }
    public Inspector? Inspector { get; set; }
    public List<Violation> Violations { get; set; } = new();
}
