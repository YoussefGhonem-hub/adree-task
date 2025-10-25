using InspectionVisits.Api.Utils;
using InspectionVisits.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InspectionVisits.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _um;
    private readonly SignInManager<ApplicationUser> _sm;
    private readonly IConfiguration _config;

    public AuthController(UserManager<ApplicationUser> um, SignInManager<ApplicationUser> sm, IConfiguration config)
    {
        _um = um; _sm = sm; _config = config;
    }

    public record LoginRequest(string Email, string Password);

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _um.FindByEmailAsync(req.Email);
        if (user is null) return Unauthorized();

        var valid = await _um.CheckPasswordAsync(user, req.Password);
        if (!valid) return Unauthorized();

        var roles = await _um.GetRolesAsync(user);
        var jwt = JwtTokenHelper.GenerateToken(
            user,
            roles,
            _config["Jwt:Key"] ?? "dev_secret_key_change_me",
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"]
        );

        return Ok(new { token = jwt.Token, expiresUtc = jwt.ExpiresUtc, roles });
    }
}
