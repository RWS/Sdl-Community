using System;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    [Serializable]
    public class Currency: ICloneable
    {

        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Country { get; set; }


        public Currency()
        {
            Name = string.Empty;
            Symbol = string.Empty;
            Country = string.Empty;
        }
        public Currency(string name, string symbol, string country)
        {
            Name = name;
            Symbol = symbol;
            Country = country;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
