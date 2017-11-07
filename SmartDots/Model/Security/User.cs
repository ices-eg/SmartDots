using System;

namespace SmartDots.Model
{
    public class User
    {
        public Guid ID { get; set; }
        public string AccountName { get; set; }
        public string Token { get; set; }
    }
}
