using InspectionVisits.Infrastructure.Identity;
using InspectionVisits.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InspectionVisits.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        services.AddHttpContextAccessor(); // SignInManager needs this

        services
            .AddIdentityCore<ApplicationUser>()        // core identity
            .AddRoles<ApplicationRole>()               // roles
            .AddEntityFrameworkStores<AppDbContext>()  // EF stores
            .AddSignInManager()                        // <-- add this
            .AddDefaultTokenProviders();               // tokens (reset, etc.)

        return services;
    }
}