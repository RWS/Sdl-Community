using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Sdl.Community.Productivity.Model;
using Sdl.Community.Productivity.Services.Persistence;
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

        public Document ActiveDocument { get; set; }

        public KeyboardTrackingService(Logger logger)
        {
            _logger = logger;
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

                var trackInfo = _trackingInfos.FirstOrDefault(x => x.FileId == document.ActiveFile.Id);
                if (trackInfo == null)
                {
                    trackInfo = new TrackInfo
                    {
                        FileId = document.ActiveFile.Id,
                        FileName = document.ActiveFile.Name,
                        ProjectId = projectInfo.Id,
                        ProjectName = projectInfo.Name,
                        Language = document.ActiveFile.Language.CultureInfo.Name,
                        FileType = document.ActiveFile.FileTypeId,
                        SegmentTrackInfos = new List<SegmentTrackInfo>()
                    };
                    _trackingInfos.Add(trackInfo);
                }
                ActiveDocument = document;
          
                _persistance.Save(_trackingInfos);
            }
            catch (Exception exception)
            {
                _logger.Debug(exception, @"Error appeared when UnregisterDocument");
            }

        }

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
                throw;
            }
        }

        void document_SegmentsConfirmationLevelChanged(object sender, EventArgs e)
        {
            var segmentContainer = sender as ISegmentContainerNode;
            if (segmentContainer == null) return;

            var targetSegment = segmentContainer.Segment;
            SetTrackingElement(ActiveDocument.ActiveFile.Id, targetSegment);
            _persistance.Save(_trackingInfos);
        }

        void document_ContentChanged(object sender, DocumentContentEventArgs e)
        {
            SetTrackingElement(e.Document.ActiveFile.Id, e.Document.ActiveSegmentPair.Target);
            _persistance.Save(_trackingInfos);
            KeyboardTracking.Instance.ClearShortcuts();
        }

        private void SetTrackingElement(Guid fileId, ISegment targetSegment)
        {
            if (targetSegment == null) return;
            var trackInfo = _trackingInfos.FirstOrDefault(x => x.FileId == fileId);

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
