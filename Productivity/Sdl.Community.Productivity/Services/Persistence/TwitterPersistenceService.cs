using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Sdl.Community.Productivity.Model;

namespace Sdl.Community.Productivity.Services.Persistence
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
                    @"SDL Community\Productivity\twitter.json");
            }
        }
    }
}
