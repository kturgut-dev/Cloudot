using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cloudot.Infrastructure.Auth.Jwt;

// public class JwtTokenHelper(IConfiguration configuration) : IJwtTokenHelper
// {
//     private readonly IConfiguration _configuration = configuration;
//
//     public string GenerateToken(IEnumerable<Claim> claims, int expireHours = 8)
//     {
//         string key = _configuration["Jwt:Key"]!;
//         string issuer = _configuration["Jwt:Issuer"]!;
//
//         var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
//         var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
//
//         var descriptor = new SecurityTokenDescriptor
//         {
//             Subject = new ClaimsIdentity(claims),
//             Expires = DateTime.UtcNow.AddHours(expireHours),
//             SigningCredentials = credentials,
//             Issuer = issuer,
//             Audience = issuer
//         };
//
//         var handler = new JwtSecurityTokenHandler();
//         var token = handler.CreateToken(descriptor);
//         return handler.WriteToken(token);
//     }
//
//     public ClaimsPrincipal? ValidateToken(string token)
//     {
//         string key = _configuration["Jwt:Key"]!;
//         string issuer = _configuration["Jwt:Issuer"]!;
//
//         var parameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidIssuer = issuer,
//             ValidateAudience = true,
//             ValidAudience = issuer,
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
//             ValidateLifetime = true,
//             ClockSkew = TimeSpan.Zero
//         };
//
//         try
//         {
//             var handler = new JwtSecurityTokenHandler();
//             return handler.ValidateToken(token, parameters, out _);
//         }
//         catch
//         {
//             return null;
//         }
//     }
// } 