using InspectionVisits.Domain.Entities;
using InspectionVisits.Infrastructure.Identity;
using InspectionVisits.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InspectionVisits.Infrastructure.Seed;
public class DbSeeder
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext db, UserManager<ApplicationUser> um, RoleManager<ApplicationRole> rm, ILogger<DbSeeder> logger)
    {
        _db = db; _userManager = um; _roleManager = rm; _logger = logger;
    }

    public async Task SeedAsync()
    {
        await _db.Database.MigrateAsync();

        foreach (var role in new[] { "Admin", "Inspector" })
        {
            if (!await _roleManager.Roles.AnyAsync(r => r.Name == role))
                await _roleManager.CreateAsync(new ApplicationRole { Name = role });
        }

        // Admin user
        var adminEmail = "admin@demo.local";
        var admin = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FullName = "Demo Admin" };
            await _userManager.CreateAsync(admin, "P@ssw0rd!");
            await _userManager.AddToRoleAsync(admin, "Admin");
        }

        // Inspector user
        var inspEmail = "inspector@demo.local";
        var insp = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == inspEmail);
        if (insp == null)
        {
            insp = new ApplicationUser { UserName = inspEmail, Email = inspEmail, EmailConfirmed = true, FullName = "Demo Inspector" };
            await _userManager.CreateAsync(insp, "P@ssw0rd!");
            await _userManager.AddToRoleAsync(insp, "Inspector");
        }

        if (!await _db.Inspectors.AnyAsync())
        {
            _db.Inspectors.AddRange(
                new Inspector { FullName = "Ahmed Ali", Email = "ahmed@org.com", Phone = "0100000001", Role = "Inspector" },
                new Inspector { FullName = "Mona Youssef", Email = "mona@org.com", Phone = "0100000002", Role = "Inspector" }
            );
        }

        if (!await _db.EntitiesToInspect.AnyAsync())
        {
            _db.EntitiesToInspect.AddRange(
                new EntityToInspect { Name = "Blue Cafe", Address = "12 Nile St, Cairo", Category = "Food" },
                new EntityToInspect { Name = "Fresh Market", Address = "22 Tahrir Sq, Cairo", Category = "Retail" }
            );
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Database seeded.");
    }
}
