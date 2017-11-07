namespace SmartDots.Model.Extension
{
    public class ConnectionInfo
    {
        public string ApiUrl { get; set; }
        public string AuthenticationType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool ConnectionSucceeded => string.IsNullOrWhiteSpace(ErrorMessage);
        public string ErrorMessage { get; set; }
    }
}
