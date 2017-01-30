using System;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class ClientActivityType: ICloneable
    {

        public string Id { get; set; }
        public string IdActivity { get; set; }
        public decimal HourlyRateClient { get; set; }
        public decimal HourlyRateAdjustment { get; set; }
        public bool Activated { get; set; }


        public ClientActivityType()
        {
            Id = string.Empty;
            IdActivity = string.Empty;
            HourlyRateClient = 0;
            HourlyRateAdjustment = 0;
            Activated = true;

        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
