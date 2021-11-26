namespace MiaCore
{
    public class MiaCoreOptions
    {
        public string ConnectionString { get; set; }
        public string JwtSecret { get; set; }
        public bool UseLoginEndpoint { get; set; } = true;
        public bool UseRegisterEndpoint { get; set; } = true;
        public bool UseRecoveryPasswordEndpoint { get; set; } = true;
        public bool UseUserListEndpoint { get; set; } = true;
        public bool UseCreateUserEndpoint { get; set; } = true;
        public bool UseLoginFirebaseEndpoint { get; set; } = true;
    }
}