using System.Net;

namespace MiaCore.Exceptions
{
    public class ForbiddenException : MiaCoreException
    {
        public ForbiddenException() : base(ErrorMessages.NoAccessToResource, HttpStatusCode.Forbidden)
        {
        }
    }
}