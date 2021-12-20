using System.Collections.Generic;

namespace MiaCore.Exceptions
{
    public static class ErrorMessages
    {
        public static KeyValuePair<int, string> IncorrectPassword { get; } = new KeyValuePair<int, string>(-3, "Password is not correct");
        public static KeyValuePair<int, string> EmailAlreadyExists { get; } = new KeyValuePair<int, string>(-4, "Email alraedy exists");
        public static KeyValuePair<int, string> InvalidFirebaseToken { get; } = new KeyValuePair<int, string>(-5, "Invalid Firebase token");
        public static KeyValuePair<int, string> UserIsNotAuthenticated { get; } = new KeyValuePair<int, string>(-6, "User is not authenticated");
        public static KeyValuePair<int, string> ValidationFailed { get; } = new KeyValuePair<int, string>(-7, "Validation Failed");
        public static KeyValuePair<int, string> ResourceNotFound { get; } = new KeyValuePair<int, string>(-8, "{0} not found");
        public static KeyValuePair<int, string> NoAccessToResource { get; } = new KeyValuePair<int, string>(-9, "You don't have access to this resource");
    }
}