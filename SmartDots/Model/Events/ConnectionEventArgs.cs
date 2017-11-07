using System;
using SmartDots.Model.Extension;

namespace SmartDots.Model.Events
{
    public class ConnectionEventArgs : EventArgs
    {
        public ConnectionInfo ConnectionInfo { get; set; }
        public User UserInfo { get; set; }
    }
}
