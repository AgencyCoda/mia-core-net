using System;
using System.Collections.Generic;
using System.Net;

namespace MiaCore.Exceptions
{
    public class MiaCoreException : Exception
    {
        public MiaCoreException(int code, string message, HttpStatusCode statusCode) : base(message)
        {
            Code = code;
            StatusCode = statusCode;
        }

        public MiaCoreException(KeyValuePair<int, string> error, HttpStatusCode statusCode) : this(error.Key, error.Value, statusCode)
        {
        }

        public int Code { get; private set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}