using System;
using System.Collections.Generic;
using System.IO;
using NLog;
using Sdl.Community.Productivity.Model;

namespace Sdl.Community.Productivity.Services.Persistence
{
    public class TrackInfoPersistanceService : AbstractPersistenceService<List<TrackInfo>>
    {
        public TrackInfoPersistanceService(Logger logger) : base(logger)
        {

        }

        public override string PersistencePath
        {
            get
            {
                var currentDate = DateTime.UtcNow;
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    string.Format(@"SDL Community\Productivity\productivity-{0}-{1}.json",currentDate.Year, currentDate.Month));
            }
        }

    }
}
