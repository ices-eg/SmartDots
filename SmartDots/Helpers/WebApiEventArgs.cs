using System;

namespace SmartDots.Helpers
{
    public class WebApiEventArgs : EventArgs
    {
        public string Command { get; set; }
        public object Object { get; set; }
        public Exception Error { get; set; }
    }
}
