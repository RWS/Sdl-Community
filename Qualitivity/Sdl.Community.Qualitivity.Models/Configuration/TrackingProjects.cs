using System;
using System.Collections.Generic;
using Sdl.Community.Structures.Projects;

namespace Sdl.Community.Structures.Configuration
{
    [Serializable]
    public class TrackingProjects : ICloneable
    {
       
        public List<Project> TrackerProjects { get; set; }

        public TrackingProjects()
        {
           
            TrackerProjects = new List<Project>();
        }

        public object Clone()
        {

            var trackingProjects = new TrackingProjects {TrackerProjects = new List<Project>()};

            foreach (var tp in TrackerProjects)
                trackingProjects.TrackerProjects.Add((Project)tp.Clone());

            return trackingProjects;

        }
    }
}
