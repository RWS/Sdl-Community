using System;
using System.Collections.Generic;
using Sdl.Community.Structures.iProperties;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class GeneralSettings : ICloneable
    {

        public List<GeneralProperty> GeneralProperties { get; set; }
        public GeneralSettings()
        {
            GeneralProperties = new List<GeneralProperty>();
        }

        public object Clone()
        {
            var generalSettings = new GeneralSettings {GeneralProperties = new List<GeneralProperty>()};

            foreach (var property in GeneralProperties)
                generalSettings.GeneralProperties.Add((GeneralProperty)property.Clone());


            return generalSettings;
        }


     
    }
}
