using System;

namespace SmartDots.Model
{
    [Serializable]
    public class LarvaeDevelopmentStage
    {
        public Guid ID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
