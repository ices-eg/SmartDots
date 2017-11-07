using System;

namespace SmartDots.Model
{
    public class Folder
    {
        public Guid ID { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
