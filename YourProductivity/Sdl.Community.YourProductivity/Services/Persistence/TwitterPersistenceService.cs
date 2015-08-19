using System;
using System.IO;
using NLog;
using Sdl.Community.YourProductivity.Model;

namespace Sdl.Community.YourProductivity.Services.Persistence
{
    public class TwitterPersistenceService : AbstractPersistenceService<TwitterAccountInfo>
    {
        public TwitterPersistenceService(Logger logger) : base(logger)
        {
        }

        public bool HasAccountConfigured
        {
            get { return File.Exists(PersistencePath); }
        }

        public override string PersistencePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"SDL Community\YourProductivity\twitter.json");
            }
        }
    }
}
