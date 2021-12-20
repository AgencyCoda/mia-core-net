using System.Net;

namespace MiaCore.Exceptions
{
    public class ResourceNotFoundException : MiaCoreException
    {
        public ResourceNotFoundException(string resource) : base(ErrorMessages.ResourceNotFound.Key, string.Format(ErrorMessages.ResourceNotFound.Value, resource), HttpStatusCode.BadRequest)
        {
        }
    }
}