using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NLog;
using Sdl.Community.YourProductivity.Persistance;
using Sdl.Community.YourProductivity.Persistance.Model;
using Sdl.Community.YourProductivity.Persistence;
using Sdl.Community.YourProductivity.Util;
using Sdl.Core.Globalization;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.YourProductivity.Services
{
	public class KeyboardTrackingService
    {

        private readonly List<TrackInfo> _trackingInfos; 
        private readonly TrackInfoDb db;
        private readonly Logger _logger;
        private Document _activeDocument;
        private object lockObject = new object();
        private Timer _timer;

        public Document ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if (value != null && value.Mode == EditingMode.Translation)
                {
                    KeyboardTracking.Instance.Activate();
                }
                else
                {
                    KeyboardTracking.Instance.Deactivate();
                }
                _activeDocument = value;
            }
        }

        public KeyboardTrackingService(Logger logger)
        {
            _logger = logger;
            db = new TrackInfoDb();
            _trackingInfos = new List<TrackInfo>();
            _timer = new Timer(AutoSave);
        }

        public async void RegisterDocument(Document document)
        {
            try
            {
                document.ContentChanged += document_ContentChanged;
                document.SegmentsConfirmationLevelChanged += document_SegmentsConfirmationLevelChanged;

                var newTrackInfo = new List<TrackInfo>();
                var projectInfo = document.Project.GetProjectInfo();
                foreach (var file in document.Files)
                {
                    _logger.Info(string.Format("Registered file: {0}", file.Name));
                    var trackInfo =await db.GetTrackInfoByFileIdAsync(file.Id, RavenContext.Current.CurrentSession);
                        
                    if (trackInfo == null)
                    {
                        trackInfo = new TrackInfo
                        {
                            FileId = file.Id,
                            FileName = file.Name,
                            ProjectId = projectInfo.Id,
                            ProjectName = projectInfo.Name,
                            Language = file.Language.CultureInfo.Name,
                            FileType = file.FileTypeId
                        };
                        newTrackInfo.Add(trackInfo);
                    }
                    _trackingInfos.Add(trackInfo);

                }

                ActiveDocument = document;
                await db.AddTrackInfosAsync(newTrackInfo, RavenContext.Current.CurrentSession);
                _timer.Change(120000, 60000);

            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when RegisterDocument");
            }
        }

        private async void EnsureSessionIsNotOld()
        {
            if (RavenContext.Current.CurrentSession.Advanced.NumberOfRequests >= 25)
            {
                var fileIds = _trackingInfos.Select(x => x.FileId).ToList();
                lock (lockObject)
                {
                    db.SaveChangesAsync(RavenContext.Current.CurrentSession);
                    RavenContext.Current.CloseCurrentSession();
                    _trackingInfos.Clear();
                }
                foreach (var file in fileIds)
                {
                    var trackInfo = await db.GetTrackInfoByFileIdAsync(file, RavenContext.Current.CurrentSession);
                    _trackingInfos.Add(trackInfo);
                }
            }
        }
        private void AutoSave(object state)
        {
            EnsureSessionIsNotOld();
            lock (lockObject)
            {
                db.SaveChangesAsync(RavenContext.Current.CurrentSession);
            }
        }

        /// <summary>
        /// Make sure keys are registered against the segment since last change and saves the information
        /// on the disk
        /// </summary>
        /// <param name="document"></param>
        public async void UnregisterDocument(Document document)
        {
            try
            {
                var keyStrokes = KeyboardTracking.Instance.GetCount();
                if (document.ActiveSegmentPair != null)
                {
                    SetTrackingElement(document.ActiveFile.Id, document.ActiveSegmentPair.Target, keyStrokes);
                }
                EnsureSessionIsNotOld();
                lock (lockObject)
                {
                    db.SaveChangesAsync(RavenContext.Current.CurrentSession);
                }
                foreach (var file in document.Files)
                {
                    _trackingInfos.RemoveAll(x => x.FileId == file.Id);

                }
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when UnregisterDocument");
            }
        }

        private void document_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            try
            {
                var segmentContainer = sender as ISegmentContainerNode;
                if (segmentContainer == null) return;

                if (ActiveDocument == null)
                {
                    _logger.Error("Segments confirmation level active document is null");

                    return;
                }

                var file = ActiveDocument.TryGetActiveFile();
                
                if (file == null)
                {
                    _logger.Error(string.Format("Segments confirmation level active document has no active file but has {0} files part of it.",ActiveDocument.Files.Count()));
                    return;
                }
                var targetSegment = segmentContainer.Segment;
                var keyStrokes = KeyboardTracking.Instance.GetCount();

                SetTrackingElement(file.Id, targetSegment, keyStrokes, false);
            }
            catch (Exception exception)
            {
                _logger.Debug(exception);
            }
        }

        private void document_ContentChanged(object sender, DocumentContentEventArgs e)
        {
            try
            {
                var keyStrokes = KeyboardTracking.Instance.GetCount();
                if (ActiveDocument == null)
                {
                    _logger.Error("ContentChanged level active document is null");

                    return;
                }
                var file = ActiveDocument.TryGetActiveFile();
                if (file == null)
                {
                    _logger.Error(string.Format("ContentChanged level active document has no active file but has {0} files part of it.", ActiveDocument.Files.Count()));
                    return;
                }
                SetTrackingElement(file.Id, e.Document.ActiveSegmentPair.Target, keyStrokes);
                KeyboardTracking.Instance.ClearShortcuts();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
            }
        }

        private void SetTrackingElement(Guid fileId,
            ISegment targetSegment,
            int keyStrokes,
            bool shouldCreateSegmentTrackInfo = true)
        {
            if (targetSegment == null) return;
            var trackInfo = _trackingInfos.Find(x => x.FileId == fileId);

            if (trackInfo == null)
            {
                _logger.Error("No track info for file {0} and segment {1}", fileId, targetSegment);
                return;
            }

            CreateUpdateTrackInfo(trackInfo, targetSegment,keyStrokes,shouldCreateSegmentTrackInfo);
        }

        private void CreateUpdateTrackInfo(TrackInfo trackInfo,
            ISegment targetSegment,
            int keyStrokes,
            bool shouldCreateSegmentTrackInfo)
        {
            if (targetSegment == null) return;
            var segmentText = targetSegment.ToString();

            if (keyStrokes == 0 && segmentText.Length == 0) return;

            var segment = GetOrCreateSegmentTrackInfo(trackInfo, targetSegment, shouldCreateSegmentTrackInfo);
            if (segment == null) return;
            var translated = targetSegment.Properties.ConfirmationLevel ==
                                                  ConfirmationLevel.Translated;
            segment.SetTrackInfo(keyStrokes, targetSegment.ToString(), translated);
        }

        private SegmentTrackInfo GetOrCreateSegmentTrackInfo(TrackInfo trackInfo,
            ISegment targetSegment,
            bool shouldCreate)
        {
            if (trackInfo.SegmentTrackInfos == null)
            {
                var exceptionMessage =
                    string.Format("SegmentTrackInfos is null for project {0}  and segment {1}!",
                        trackInfo.ProjectName, targetSegment);
                throw new ArgumentNullException(exceptionMessage);
            }

            if (targetSegment == null)
            {
                var exceptionMessage =
                    string.Format("Segment is null for {0}!",
                        trackInfo.ProjectName);
                throw new ArgumentNullException(exceptionMessage);
            }

            if (targetSegment.Properties == null)
            {
                var exceptionMessage =
                    string.Format("Segment properties are null for {0}!",
                        trackInfo.ProjectName);
                throw new ArgumentNullException(exceptionMessage);
            }

            SegmentTrackInfo segment = trackInfo.SegmentTrackInfos.Find(x => x.SegmentId == targetSegment.Properties.Id.Id);

            if (segment == null && shouldCreate)
            {
                var segmentId = targetSegment.Properties.Id.Id;

                segment = new SegmentTrackInfo {SegmentId = segmentId};
                trackInfo.SegmentTrackInfos.Add(segment);
            }
            return segment;
        }
    }
}
