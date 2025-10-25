using InspectionVisits.Domain.Common;

namespace InspectionVisits.Domain.Entities;
public class Inspector : BaseEntity
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Role { get; set; } = "Inspector";
}
