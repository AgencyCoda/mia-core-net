using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using MiaCore.JWT;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace MiaCor.UnitTests.JWT
{
    public class JwtHelperTests
    {
        [Fact]
        public void DecodeTokenShouldReturnClaimsWhenTokenIsValid()
        {
            var secret = "KuUGbRFYEKD5zVKuAif6Ous81xPxusgr";
            var claims = new Dictionary<string, string>()
            {
                {"key","value"},
                {"key1","value1"},
            };
            var token = generateToken(secret, claims, 1000);

            var result = JwtHelper.DecodeToken(token, secret);

            result.Should().ContainKeys("key", "key1");
        }

        [Fact]
        public void DecodeTokenShouldThrowJwtTokenValidationExceptionWhenSecretIsNotValid()
        {
            var secret = "KuUGbRFYEKD5zVKuAif6Ous81xPxusgr";
            var incorrecSecret = "1234455";
            var claims = new Dictionary<string, string>()
            {
                {"key","value"},
            };
            var token = generateToken(secret, claims, 1000);

            Action act = () => JwtHelper.DecodeToken(token, incorrecSecret);

            act.Should().Throw<JwtTokenValidationException>();
        }

        [Fact]
        public void DecodeTokenShouldThrowJwtTokenValidationExceptionWhenTokenIsNotValid()
        {
            var secret = "KuUGbRFYEKD5zVKuAif6Ous81xPxusgr";
            var claims = new Dictionary<string, string>()
            {
                {"key","value"},
            };
            var token = generateToken(secret, claims, 1000);
            token = token + "a"; // add symbol to token so it should not be valid

            Action act = () => JwtHelper.DecodeToken(token, secret);

            act.Should().Throw<JwtTokenValidationException>();
        }

        [Fact]
        public void DecodeTokenShouldThrowJwtTokenValidationExceptionWhenTokenIsExpired()
        {
            var secret = "KuUGbRFYEKD5zVKuAif6Ous81xPxusgr";
            var claims = new Dictionary<string, string>()
            {
                {"key","value"},
            };
            var token = generateToken(secret, claims, 1);
            System.Threading.Thread.Sleep(2);

            Action act = () => JwtHelper.DecodeToken(token, secret);

            act.Should().Throw<JwtTokenValidationException>();
        }

        private string generateToken(string secret, Dictionary<string, string> claims, int expInMilliseconds)
        {
            var keyBytes = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value))),
                Expires = DateTime.UtcNow.AddMilliseconds(expInMilliseconds),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}