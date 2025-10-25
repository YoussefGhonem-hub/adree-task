// src/InspectionVisits.Api/Utils/JwtTokenHelper.cs
using InspectionVisits.Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InspectionVisits.Api.Utils;

/// <summary>
/// Helper for issuing JWT access tokens.
/// </summary>
public static class JwtTokenHelper
{
    /// <summary>
    /// Strongly-typed result for a generated JWT.
    /// </summary>
    public sealed record JwtResult(string Token, DateTime ExpiresUtc);

    /// <summary>
    /// Generate a JWT for the given user and roles.
    /// Values for issuer/audience are optional and can be null/empty if you don't validate them.
    /// </summary>
    /// <param name="user">Identity user</param>
    /// <param name="roles">User roles</param>
    /// <param name="signingKey">Symmetric signing key (raw string from configuration)</param>
    /// <param name="expiresInHours">Expiry in hours (default 8)</param>
    /// <param name="issuer">Optional token issuer</param>
    /// <param name="audience">Optional token audience</param>
    /// <param name="additionalClaims">Optional extra claims to include</param>
    public static JwtResult GenerateToken(
        ApplicationUser user,
        IEnumerable<string> roles,
        string signingKey,
        int expiresInHours = 8,
        string? issuer = null,
        string? audience = null,
        IEnumerable<Claim>? additionalClaims = null)
    {
        if (string.IsNullOrWhiteSpace(signingKey))
            throw new ArgumentException("Signing key must be provided.", nameof(signingKey));

        var nowUtc = DateTime.UtcNow;
        var expiresUtc = nowUtc.AddHours(expiresInHours);

        // Base identity claims
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            // helpful standard JWT claims
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, ToUnixEpoch(nowUtc).ToString(), ClaimValueTypes.Integer64)
        };

        // Roles
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        // Any extra claims (e.g., FullName, custom IDs, etc.)
        if (additionalClaims is not null)
            claims.AddRange(additionalClaims);

        // Create signing credentials
        var signingCredentials = CreateSigningCredentials(signingKey);

        // Build token
        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(issuer) ? null : issuer,
            audience: string.IsNullOrWhiteSpace(audience) ? null : audience,
            claims: claims,
            notBefore: nowUtc,
            expires: expiresUtc,
            signingCredentials: signingCredentials
        );

        var handler = new JwtSecurityTokenHandler();
        var tokenString = handler.WriteToken(token);

        return new JwtResult(tokenString, expiresUtc);
    }

    /// <summary>
    /// Create signing credentials from a symmetric key string using HMAC-SHA256.
    /// </summary>
    public static SigningCredentials CreateSigningCredentials(string signingKey)
    {
        var keyBytes = Encoding.UTF8.GetBytes(signingKey);
        var secKey = new SymmetricSecurityKey(keyBytes);
        return new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);
    }

    /// <summary>
    /// Convert a UTC DateTime to unix epoch seconds.
    /// </summary>
    private static long ToUnixEpoch(DateTime utc)
        => (long)Math.Round((utc - DateTime.UnixEpoch).TotalSeconds);
}
