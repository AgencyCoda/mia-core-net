using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MiaCor.UnitTests")]
namespace MiaCore.JWT
{
    internal class JwtTokenValidationException : Exception
    {
        public JwtTokenValidationException() : base()
        {

        }
        public JwtTokenValidationException(string message) : base(message)
        {

        }
    }
}