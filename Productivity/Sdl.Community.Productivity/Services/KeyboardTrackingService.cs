using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;
using Sdl.Community.Productivity.Util;
using Sdl.Core.Globalization;
using Sdl.DesktopEditor.EditorApi;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.Productivity.Services
{
    public class KeyboardTrackingService
    {

        private readonly List<TrackInfo> _trackingInfos; 
        private readonly TrackInfoPersistanceService _persistance;
        private readonly Logger _logger;
        private readonly EmailService _emailService;
        private Document _activeDocument;
        public Document ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                if (value.Mode == EditingMode.Translation)
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

        public KeyboardTrackingService(Logger logger, EmailService emailService)
        {
            _logger = logger;
            _emailService = emailService;
            _persistance = new TrackInfoPersistanceService(_logger);
            _trackingInfos = _persistance.Load();
        }

        public void RegisterDocument(Document document)
        {
            try
            {
                document.ContentChanged += document_ContentChanged;
                document.SegmentsConfirmationLevelChanged += document_SegmentsConfirmationLevelChanged;
                var projectInfo = document.Project.GetProjectInfo();

                foreach (var file in document.Files)
                {
                    _logger.Info(string.Format("Registered file: {0}", file.Name));

                    var trackInfo = _trackingInfos.FirstOrDefault(x => x.FileId == file.Id);
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
                        _trackingInfos.Add(trackInfo);
                    }
                }
               
                ActiveDocument = document;
          
                _persistance.Save(_trackingInfos);
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when RegisterDocument");
                _emailService.SendLogFile();
            }

        }

        /// <summary>
        /// Make sure keys are registered against the segment since last change and saves the information
        /// on the disk
        /// </summary>
        /// <param name="document"></param>
        public void UnregisterDocument(Document document)
        {
            try
            {
                SetTrackingElement(document.ActiveFile.Id, document.ActiveSegmentPair.Target);
                _persistance.Save(_trackingInfos);

            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when UnregisterDocument");
                _emailService.SendLogFile();

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
                    _emailService.SendLogFile();

                    return;
                }

                var file = ActiveDocument.TryGetActiveFile();
                
                if (file == null)
                {
                    _logger.Error(string.Format("Segments confirmation level active document has no active file but has {0} files part of it.",ActiveDocument.Files.Count()));
                    _emailService.SendLogFile();
                    return;
                }
                var targetSegment = segmentContainer.Segment;
                SetTrackingElement(file.Id, targetSegment);
                _persistance.Save(_trackingInfos);
            }
            catch (Exception exception)
            {
                _logger.Debug(exception);
                _emailService.SendLogFile();
            }
        }

        private void document_ContentChanged(object sender, DocumentContentEventArgs e)
        {
            try
            {
                if (ActiveDocument == null)
                {
                    _logger.Error("ContentChanged level active document is null");
                    _emailService.SendLogFile();

                    return;
                }
                var file = ActiveDocument.TryGetActiveFile();
                if (file == null)
                {
                    _logger.Error(string.Format("ContentChanged level active document has no active file but has {0} files part of it.", ActiveDocument.Files.Count()));
                    _emailService.SendLogFile();
                    return;
                }
                SetTrackingElement(file.Id, e.Document.ActiveSegmentPair.Target);
                _persistance.Save(_trackingInfos);
                KeyboardTracking.Instance.ClearShortcuts();
            }
            catch (Exception ex)
            {
                _logger.Debug(ex);
                _emailService.SendLogFile();

            }
        }

        private void SetTrackingElement(Guid fileId, ISegment targetSegment)
        {
            if (targetSegment == null) return;
            var trackInfo = _trackingInfos.FirstOrDefault(x => x.FileId == fileId);

            if (trackInfo == null)
            {
                _logger.Error("No track info for file {0} and segment {1}", fileId, targetSegment);
                return;
            }

            CreateUpdateTrackInfo(trackInfo, targetSegment);
        }

        private void CreateUpdateTrackInfo(TrackInfo trackInfo, ISegment targetSegment)
        {
            if (targetSegment == null) return;
            var keyStrokes = KeyboardTracking.Instance.GetCount();
            var segmentText = targetSegment.ToString();

            if (keyStrokes == 0 && segmentText.Length == 0) return;

            var segment = GetOrCreateSegmentTrackInfo(trackInfo, targetSegment);
            var translated = targetSegment.Properties.ConfirmationLevel ==
                                                  ConfirmationLevel.Translated;
            segment.SetTrackInfo(keyStrokes, targetSegment.ToString(), translated);
        }

        private SegmentTrackInfo GetOrCreateSegmentTrackInfo(TrackInfo trackInfo, ISegment targetSegment)
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

            var segment = trackInfo.SegmentTrackInfos.Find(x => x.SegmentId == targetSegment.Properties.Id.Id);

            if (segment == null)
            {
                var segmentId = targetSegment.Properties.Id.Id;

                segment = new SegmentTrackInfo {SegmentId = segmentId};
                trackInfo.SegmentTrackInfos.Add(segment);
            }
            return segment;
        }
    }
}
