namespace SmartDots.Helpers
{
    public class WebApiResult
    {
        public dynamic Result { get; set; }
        public string ErrorMessage { get; set; }
        public bool Succeeded => string.IsNullOrWhiteSpace(ErrorMessage);
    }

    public class WebApiResult<t>
    {
        public t Result { get; set; }
        public string ErrorMessage { get; set; }
        public bool Succeeded => string.IsNullOrWhiteSpace(ErrorMessage);
    }
}
