using System;

namespace MiaCore.JWT
{
    public class JwtTokenValidationException : Exception
    {
        public JwtTokenValidationException() : base()
        {

        }
        public JwtTokenValidationException(string message) : base(message)
        {

        }
    }
}