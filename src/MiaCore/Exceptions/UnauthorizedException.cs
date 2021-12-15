using System.Collections.Generic;
using System.Net;

namespace MiaCore.Exceptions
{
    public class UnauthorizedException : MiaCoreException
    {
        public UnauthorizedException(KeyValuePair<int, string> error) : base(error.Key, error.Value, HttpStatusCode.Unauthorized)
        {

        }
    }
}