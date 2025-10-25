using FluentValidation;
using InspectionVisits.Application.Common.Behaviors;
using InspectionVisits.Application.Features.Entities.Commands;
using InspectionVisits.Application.Mappings;
using InspectionVisits.Infrastructure;
using InspectionVisits.Infrastructure.Identity;
using InspectionVisits.Infrastructure.Persistence;
using InspectionVisits.Infrastructure.Seed;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/inspectionvisits-.log", rollingInterval: RollingInterval.Day));

// Db + Identity + Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// MediatR + FluentValidation + AutoMapper
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateEntityToInspect>();
});
builder.Services.AddCors();   // no named policies

builder.Services.AddValidatorsFromAssemblyContaining<CreateEntityToInspect>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Auth: JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev_secret_key_change_me";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("InspectorOnly", p => p.RequireRole("Inspector"));
});

// Controllers + ProblemDetails
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    // optional: keep it forgiving when reading
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});


builder.Services.AddProblemDetails(o =>
{
    o.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Define the Bearer auth scheme
    var jwtScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme. Example: **Bearer eyJhbGciOi...**",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    // Register the scheme
    c.AddSecurityDefinition("Bearer", jwtScheme);

    // Require Bearer token by default (you can scope this per-operation if you want)
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();
app.UseCors(cors => cors
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
);

// Global exception to ProblemDetails (RFC7807)
app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext http) =>
{
    var feature = http.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
    var ex = feature?.Error;

    if (ex is FluentValidation.ValidationException vex)
    {
        var errors = vex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

        return Results.Problem(
            title: "Validation Failed",
            statusCode: StatusCodes.Status400BadRequest,
            type: "https://tools.ietf.org/html/rfc7807",
            extensions: new Dictionary<string, object?> { { "errors", errors } });
    }

    var status = ex switch
    {
        KeyNotFoundException => StatusCodes.Status404NotFound,
        InvalidOperationException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    return Results.Problem(
        title: ex?.Message ?? "An error occurred",
        statusCode: status,
        type: "https://tools.ietf.org/html/rfc7807");
});


app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.ConfigObject.AdditionalItems["persistAuthorization"] = true;
});

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map attribute-routed controllers
app.MapControllers();

// Seed DB
using (var scope = app.Services.CreateScope())
{
    var seeder = new DbSeeder(
        scope.ServiceProvider.GetRequiredService<AppDbContext>(),
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>(),
        scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>(),
        scope.ServiceProvider.GetRequiredService<ILogger<DbSeeder>>());
    await seeder.SeedAsync();
}
await app.RunAsync();


/* Auth & feature minimal endpoints removed in favor of Controllers
// (moved to AuthController) app.MapPost("/api/auth/login", async (UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, IConfiguration config, LoginRequest req) =>
{
    var user = await um.FindByEmailAsync(req.Email);
    if (user is null) return Results.Unauthorized();
    var passOk = await sm.CheckPasswordSignInAsync(user, req.Password, false);
    if (!passOk.Succeeded) return Results.Unauthorized();

    var roles = await um.GetRolesAsync(user);
    var token = JwtTokenHelper.GenerateToken(user, roles, config["Jwt:Key"] ?? "dev_secret_key_change_me");

    return Results.Ok(new { token, roles });
}).WithTags("Auth").WithOpenApi();

// Entities group removed (now in EntitiesController)
// var entities = ... (migrated)
//entities.MapGet("", async (IMediator m, string? category) => await m.Send(new ListEntitiesToInspect(category)));
//entities.MapPost("", async (IMediator m, CreateEntityToInspect req) => await m.Send(req));
//entities.MapPut("{id:int}", async (IMediator m, int id, UpdateEntityToInspectBody body) => 
    await m.Send(new UpdateEntityToInspect(id, body.Name, body.Address, body.Category)));
//entities.MapDelete("{id:int}", async (IMediator m, int id) => { await m.Send(new InspectionVisits.Application.Features.Entities.Commands.DeleteEntityToInspect(id)); return Results.NoContent(); });

// Visits workflow moved to VisitsController
// var visits = ... (migrated)
//visits.MapGet("", async (IMediator m, DateTime? from, DateTime? to, InspectionVisits.Domain.Enums.VisitStatus? status, int? inspectorId, string? category) 
    => await m.Send(new ListVisits(from, to, status, inspectorId, category))).RequireAuthorization();

//visits.MapPost("/schedule", async (IMediator m, ScheduleVisit req) => await m.Send(req)).RequireAuthorization("AdminOnly");
//visits.MapPost("/{id:int}/status", async (IMediator m, int id, UpdateVisitStatusBody body) => 
    await m.Send(new UpdateVisitStatus(id, body.NewStatus, body.Score, body.Notes))).RequireAuthorization("InspectorOnly");
//visits.MapPost("/{id:int}/violations", async (IMediator m, int id, AddViolationBody body) => 
    await m.Send(new AddViolation(id, body.Code, body.Description, body.Severity))).RequireAuthorization("InspectorOnly");

// Dashboard moved to DashboardController
//app.MapGet("/api/dashboard", async (IMediator m) => await m.Send(new GetDashboard())).RequireAuthorization();

app.Run();

// Request bodies
public record LoginRequest(string Email, string Password);
public record UpdateEntityToInspectBody(string Name, string Address, string Category);
public record UpdateVisitStatusBody(InspectionVisits.Domain.Enums.VisitStatus NewStatus, int? Score, string? Notes);
public record AddViolationBody(string Code, string Description, InspectionVisits.Domain.Enums.ViolationSeverity Severity);

public static class JwtTokenHelper
{
    public static string GenerateToken(ApplicationUser user, IList<string> roles, string key)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var identity = new System.Security.Claims.ClaimsIdentity(new[] {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName ?? ""),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email ?? ""),
        }.Concat(roles.Select(r => new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, r))));

        var token = handler.CreateJwtSecurityToken(
            subject: identity,
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(key)),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256),
            expires: DateTime.UtcNow.AddHours(8));

        return handler.WriteToken(token);
    }
}

*/
