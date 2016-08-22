using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using NLog;
using Sdl.Community.YourProductivity.Persistance.Model;
using System.Threading.Tasks;
using Sdl.Community.YourProductivity.Persistance;
using Raven.Client.Linq;
using Raven.Client;
using System.Threading;

namespace Sdl.Community.YourProductivity.Persistence
{
    public class TrackInfoDb : IDisposable
    {
        private bool disposed;
        private bool _autoSave;
        
        public TrackInfoDb()
        {

        }


        public Task<List<TrackInfo>> GetTrackInfosAsync()
        {
            this.ThrowIfDisposed();
            List<TrackInfo> allTrackInfos = new List<TrackInfo>();

            using (var session = RavenContext.Current.CreateSession())
            {
                int start = 0;
                while (true)
                {
                    var current = session.Query<TrackInfo>().Take(1024).Skip(start).ToList();
                    if (current.Count == 0)
                        break;

                    start += current.Count;
                    allTrackInfos.AddRange(current);

                }
            }

            return Task.FromResult(allTrackInfos);
            
        }
        public Task<TrackInfo> GetTrackInfoByFileIdAsync(Guid fileId, IDocumentSession session = null)
        {
            ThrowIfDisposed();
            TrackInfo result;
            IDocumentSession localSession = session;
            if (session == null) localSession = RavenContext.Current.CreateSession();

            result = localSession.Query<TrackInfo, TrackInfo_ByFileId>()
           .FirstOrDefault(x => x.FileId == fileId);

            if (session == null) localSession.Dispose();

            return Task.FromResult(result);
        }


        public Task AddTrackInfoAsync(TrackInfo trackInfo)
        {
            this.ThrowIfDisposed();
            if (trackInfo == null)
                throw new ArgumentNullException("trackInfo");
            using (var session = RavenContext.Current.CreateSession())
            {
                session.Store(trackInfo);
                session.SaveChanges();
            }
            return Task.FromResult(true);
        }

        public Task AddTrackInfosAsync(List<TrackInfo> trackInfos, IDocumentSession session = null)
        {
            this.ThrowIfDisposed();
            if (trackInfos == null)
                throw new ArgumentNullException("trackInfo");
            IDocumentSession localSession = session;
            if (session == null) localSession = RavenContext.Current.CreateSession();


            foreach (var trackInfo in trackInfos)
            {
                localSession.Store(trackInfo);
            }
            localSession.SaveChanges();
            if (session == null) localSession.Dispose();

            return Task.FromResult(true);
        }

        public Task SaveChangesAsync(IDocumentSession session)
        {
            this.ThrowIfDisposed();
           
            session.SaveChanges();
            
            return Task.FromResult(true);
        }

        public void Dispose()
        {
            disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException(this.GetType().Name);
        }

  

    }
}
