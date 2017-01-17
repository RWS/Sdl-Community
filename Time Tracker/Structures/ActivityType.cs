using System;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ActivityType: ICloneable
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Billable { get; set; }
        public decimal HourlyRate { get; set; }
        public string Currency { get; set; }


        public ActivityType()
        {
            Id = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            Billable = true;
            HourlyRate = 0;
            Currency = string.Empty;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
