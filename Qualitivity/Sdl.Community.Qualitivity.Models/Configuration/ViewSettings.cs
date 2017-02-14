using System;
using System.Collections.Generic;
using Sdl.Community.Structures.iProperties;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class ViewSettings: ICloneable
    {

        public List<ViewProperty> ViewProperties { get; set; }
        public ViewSettings()
        {
            ViewProperties = new List<ViewProperty>();          
        }

        public object Clone()
        {
            var viewSettings = new ViewSettings {ViewProperties = new List<ViewProperty>()};


            foreach (var property in ViewProperties)
                viewSettings.ViewProperties.Add((ViewProperty)property.Clone());

            return viewSettings;
        }
    }
}
