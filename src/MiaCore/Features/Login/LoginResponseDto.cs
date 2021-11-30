using System;

namespace MiaCore.Features.Login
{
    internal class LoginResponseDto : MiaUserDto
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
    }
}