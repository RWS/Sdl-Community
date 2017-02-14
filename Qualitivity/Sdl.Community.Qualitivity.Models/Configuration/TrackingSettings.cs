using System;
using System.Collections.Generic;
using Sdl.Community.Structures.iProperties;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class TrackingSettings : ICloneable
    {
         public List<GeneralProperty> TrackingProperties { get; set; }
         public TrackingSettings()
        {
            TrackingProperties = new List<GeneralProperty>();
        }

         public object Clone()
         {
             var trackingSettings = new TrackingSettings {TrackingProperties = new List<GeneralProperty>()};


             foreach (var property in TrackingProperties)
                 trackingSettings.TrackingProperties.Add((GeneralProperty)property.Clone());

             

             return trackingSettings;
         }

        
    }
}
