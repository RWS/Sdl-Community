using System;
using System.Globalization;
using System.Linq;
using Sdl.Community.Studio.Time.Tracker.Tracking;

namespace Sdl.Community.Studio.Time.Tracker.Structures
{
    public class Common
    {
        public static DateTime DateNull = new DateTime(1919, 1, 1, 0, 0, 0);

        public static ClientProfileInfo GetClientFromId(string id)
        {
            return Tracked.Preferences.Clients.FirstOrDefault(clientProfileInfo => string.Compare(clientProfileInfo.Id, id, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static string GetMonthName(DateTime givenDate)
        {
            var formatInfoinfo = new DateTimeFormatInfo();
            var monthName = formatInfoinfo.MonthNames;
            return monthName[givenDate.Month - 1];
        }


        public static ActivityType GetActivityType(string id)
        {
            return Tracked.Preferences.ActivitiesTypes.FirstOrDefault(type => type.Id == id);
        }


        public static TrackerProject GetProjectFromId(string id)
        {
            var trackerProject = new TrackerProject();

            foreach (var project in Tracked.Preferences.TrackerProjects)
            {
                if (project.Id != id)
                    continue;

                trackerProject = project;
                break;
            }
            return trackerProject;
        }
        
    }
}
