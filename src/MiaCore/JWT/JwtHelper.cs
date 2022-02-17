using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

[assembly: InternalsVisibleTo("MiaCor.UnitTests")]
namespace MiaCore.JWT
{
    /// <summary>
    /// Helper Class for Decoding/Validating JWT Tokens
    /// </summary>
    internal static class JwtHelper
    {
        /// <summary>
        /// Decodes JWT Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="secret">Secret Key</param>
        /// <returns>Dictionary of string,string containing Claims</returns>
        /// <exception cref="JwtTokenValidationException"></exception>
        public static Dictionary<string, string> DecodeToken(string token, string secret)
        {
            vaidateToken(token, secret);
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                return securityToken.Claims.ToDictionary(x => x.Type, x => x.Value);
            }
            catch (Exception ex)
            {
                throw new JwtTokenValidationException(ex.Message);
            }
        }

        public static string GenerateToken(string secret, int expirationMinutes, string subject, int role)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, subject),
                    new Claim("UserId", subject),
                    new Claim(ClaimTypes.Role, role ==1 ? "admin":"user"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }

        private static void vaidateToken(string token, string secret)
        {
            var mySecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = mySecurityKey,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);
            }
            catch (Exception ex)
            {
                throw new JwtTokenValidationException(ex.Message);
            }
        }
    }
}