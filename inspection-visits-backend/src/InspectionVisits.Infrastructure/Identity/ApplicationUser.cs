using Microsoft.AspNetCore.Identity;

namespace InspectionVisits.Infrastructure.Identity;
public class ApplicationUser : IdentityUser<int>
{
    public string? FullName { get; set; }
}
