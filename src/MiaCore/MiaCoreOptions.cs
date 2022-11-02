namespace MiaCore
{
    public class MiaCoreOptions
    {
        public string ConnectionString { get; set; }
        public string JwtSecret { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public string EmailFrom { get; set; }
        public string EmailFromName { get; set; }
        public string SendgridApiKey { get; set; }
        public string FirebaseCredentialsFilePath { get; set; }
        public string StorageCredentialsFilePath { get; set; }
        public string FontUrl { get; set; }
        public string FrontUrl { get; set; }
        public string BackUrl { get; set; }
        public bool UseLoginEndpoint { get; set; } = true;
        public bool UseRegisterEndpoint { get; set; } = true;
        public bool UseRecoveryPasswordEndpoint { get; set; } = true;
        public bool UseUserListEndpoint { get; set; } = true;
        public bool UseCreateUserEndpoint { get; set; } = true;
        public bool UseLoginFirebaseEndpoint { get; set; } = true;
        public bool SendPushNotifications { get; set; } = false;
    }
}