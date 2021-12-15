namespace MiaCore.Exceptions
{
    public class ValidationException : MiaCoreException
    {
        public ValidationException(string failures)
            : base(ErrorMessages.ValidationFailed.Key,
                    ErrorMessages.ValidationFailed.Value + ": " + failures,
                    System.Net.HttpStatusCode.BadRequest)
        {

        }
    }
}
