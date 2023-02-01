using System;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeEggQuality
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
