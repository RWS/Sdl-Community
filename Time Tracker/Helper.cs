using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sdl.Community.Studio.Time.Tracker.Tracking;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Studio.Time.Tracker
{
    public class Helper
    {

       
        public static Structures.TrackerProject GetTrackerProjectFromDocument(Document doc)
        {

            Structures.TrackerProject trackerProject = null;
            try
            {
                var projectInfo = doc.Project.GetProjectInfo();
                if (projectInfo != null)
                {
                    foreach (var project in Tracked.Preferences.TrackerProjects)
                    {
                        if (project.IdStudio != projectInfo.Id.ToString() &&
                            string.Compare(project.PathStudio, projectInfo.LocalProjectFolder.Trim()
                            , StringComparison.OrdinalIgnoreCase) != 0) 
                            continue;

                        trackerProject = project;
                        break;
                    }
                    if (trackerProject == null)
                    {
                        trackerProject = new Structures.TrackerProject
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = projectInfo.Name,
                            Path = projectInfo.LocalProjectFolder.Trim(),
                            IdStudio = projectInfo.Id.ToString(),
                            NameStudio = projectInfo.Name.Trim(),
                            PathStudio = projectInfo.LocalProjectFolder.Trim(),
                            ClientId = string.Empty,
                            ProjectActivities = new List<Structures.TrackerProjectActivity>(),
                            ProjectStatus = projectInfo.IsCompleted ? "Completed" : "In progress",
                            Description = projectInfo.Description ?? string.Empty,
                            DateStart = projectInfo.CreatedAt,
                            DateCreated = DateTime.Now,
                            DateDue =
                                projectInfo.DueDate ?? DateTime.Now.AddDays(7)
                        };


                        if (trackerProject.DateDue < DateTime.Now)
                            trackerProject.DateDue = DateTime.Now.AddDays(1);

                        trackerProject.DateCompleted = DateTime.Now;

                        Tracked.Preferences.TrackerProjects.Add(trackerProject);

                        Tracked.TarckerCheckNewProjectAdded = true;
                        Tracked.TarckerCheckNewProjectId = trackerProject.Id;
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }


            return trackerProject;
        }

        public static string GetUniqueName(string baseName, List<string> existingNames)
        {
            var rs = string.Empty;


            for (var i = 0; i < 1000; i++)
            {
                var newName = baseName + "_" + i.ToString().PadLeft(4, '0');
                var foundName = existingNames.Any(name => string.Compare(name, newName, StringComparison.OrdinalIgnoreCase) == 0);
                if (foundName) continue;
                    rs = newName;
                break;
            }

            return rs;
        }

        public static string GetStringFromDateTime(DateTime dateTime)
        {
            return dateTime.Year
                + "-" + dateTime.Month.ToString().PadLeft(2, '0')
                + "-" + dateTime.Day.ToString().PadLeft(2, '0')
                + "T" + dateTime.Hour.ToString().PadLeft(2, '0')
                + "." + dateTime.Minute.ToString().PadLeft(2, '0')
                + "." + dateTime.Second.ToString().PadLeft(2, '0');
        }

        public static DateTime GetDateTimeFromString(string strDateTime)
        {
            var dateTime = DateTime.Now;

            //2012-05-17
            var regex = new Regex(@"(?<x1>\d{4})\-(?<x2>\d{2})\-(?<x3>\d{2})T(?<x4>\d{2})\.(?<x5>\d{2})\.(?<x6>\d{2})", RegexOptions.IgnoreCase);

            var mRDateTime = regex.Match(strDateTime);
            if (!mRDateTime.Success) return dateTime;
            try
            {
                var yy = Convert.ToInt32(mRDateTime.Groups["x1"].Value);
                var mm = Convert.ToInt32(mRDateTime.Groups["x2"].Value);
                var dd = Convert.ToInt32(mRDateTime.Groups["x3"].Value);

                var hh = Convert.ToInt32(mRDateTime.Groups["x4"].Value);
                var MM = Convert.ToInt32(mRDateTime.Groups["x5"].Value);
                var ss = Convert.ToInt32(mRDateTime.Groups["x6"].Value);

                dateTime = new DateTime(yy, mm, dd, hh, MM, ss);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return dateTime;
        }

    }
}
