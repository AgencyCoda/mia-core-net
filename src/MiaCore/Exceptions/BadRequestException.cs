using System.Collections.Generic;
using System.Net;

namespace MiaCore.Exceptions
{
    public class BadRequestException : MiaCoreException
    {
        public BadRequestException(KeyValuePair<int, string> error) : base(error.Key, error.Value, HttpStatusCode.Unauthorized)
        {
        }
    }
}