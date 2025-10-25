using InspectionVisits.Domain.Entities;
using InspectionVisits.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InspectionVisits.Infrastructure.Persistence;
public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Inspector> Inspectors => Set<Inspector>();
    public DbSet<EntityToInspect> EntitiesToInspect => Set<EntityToInspect>();
    public DbSet<InspectionVisit> InspectionVisits => Set<InspectionVisit>();
    public DbSet<Violation> Violations => Set<Violation>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<EntityToInspect>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Address).HasMaxLength(400).IsRequired();
            e.Property(x => x.Category).HasMaxLength(100).IsRequired();
        });

        b.Entity<Inspector>(e =>
        {
            e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Email).HasMaxLength(200).IsRequired();
            e.Property(x => x.Phone).HasMaxLength(50).IsRequired();
            e.Property(x => x.Role).HasMaxLength(50).IsRequired();
        });

        b.Entity<InspectionVisit>(e =>
        {
            e.HasOne(x => x.EntityToInspect).WithMany().HasForeignKey(x => x.EntityToInspectId);
            e.HasOne(x => x.Inspector).WithMany().HasForeignKey(x => x.InspectorId);
            e.Property(x => x.Notes).HasMaxLength(2000);
        });

        b.Entity<Violation>(e =>
        {
            e.HasOne(x => x.InspectionVisit).WithMany(v => v.Violations).HasForeignKey(x => x.InspectionVisitId);
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500).IsRequired();
        });
    }
}
