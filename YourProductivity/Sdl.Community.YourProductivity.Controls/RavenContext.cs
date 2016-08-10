using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.YourProductivity.Model;
using System.Runtime.InteropServices;
using System.IO;
using Raven.Client.Embedded;
using Raven.Client;
using Sdl.Community.YourProductivity.Persistance.Model;
using Raven.Database.Config;
using Raven.Database.Server;
using Sdl.Community.YourProductivity.Persistence;
using Raven.Client.Indexes;

namespace Sdl.Community.YourProductivity.Persistance
{
    public  class RavenContext : IDisposable
    {
        private EmbeddableDocumentStore store;
        private static readonly Lazy<RavenContext> ravenContext = new Lazy<RavenContext>(() => new RavenContext());
        private IDocumentSession session;
        private RavenContext()
        {
            
            store = new EmbeddableDocumentStore
            {
                DataDirectory = "Data",
                DefaultDatabase = CurrentDatabaseName
            };
            store.Configuration.Settings["Raven/WorkingDir"] = GetWorkingDirectory();
            store.Configuration.Initialize();
            store.Initialize();
            new TrackInfo_ByFileId().Execute(store);
        }

        public static RavenContext Current { get { return ravenContext.Value; } }

        public string CurrentDatabaseName
        {
            get
            {
                var currentDate = DateTime.UtcNow;
                return string.Format(@"YourProductivity-{0}-{1}", currentDate.Year, currentDate.Month);
            }
        }
        private string GetWorkingDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                @"SDL Community\YourProductivity");
        }

        public IDocumentSession CurrentSession
        {
            get
            {
                if(session== null)
                {
                    session = store.OpenSession(); 
                }
                return session;
            }
        }

        public void CloseCurrentSession()
        {
            if (session == null) return;
            session.Dispose();
            session = null;
        }

        internal IDocumentSession CreateSession()
        {
            return store.OpenSession();
        }

        public void Initialize()
        {
            //check if there are any old databases and remove them
            var di = new DirectoryInfo(GetWorkingDirectory());
            var yourProductivityDatabases = di.GetDirectories("YourProductivity*", SearchOption.AllDirectories);

            foreach (var database in yourProductivityDatabases)
            {
                if (database.Name.Equals(CurrentDatabaseName)) continue;
                store.AsyncDatabaseCommands.GlobalAdmin.DeleteDatabaseAsync(database.Name, hardDelete: true);
            }

        }

        public void Dispose()
        {
            CloseCurrentSession();
            store.Dispose();
        }
    }

    public class TrackInfo_ByFileId: AbstractIndexCreationTask<TrackInfo>
    {
        public TrackInfo_ByFileId()
        {
            Map = trackInfos => from trackInfo in trackInfos
                                select new { FileId = trackInfo.FileId };
        }
    }
}
