using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.AntidoteVerifier
{
    public class EditorService : IEditorService
    {
        private Document _document;
        private Dictionary<long, string> _segmentMetadata;
        public EditorService(Document document)
        {
            _document = document;
            _segmentMetadata = new Dictionary<long, string>();
            Initialize();
        }

        private void Initialize()
        {
            foreach (var segmentPair in _document.SegmentPairs)
            {
                var id = long.Parse(segmentPair.Properties.Id.Id);
                var targetString = segmentPair.Target.ToString();
                _segmentMetadata.Add(id, targetString);
            }
        }

        public long GetDocumentId()
        {
            //we will only one active document at a time so this will always be 1
            return 1;
        }

        public long GetDocumentNoOfSegments()
        {
            return _document.SegmentPairs.Count();
        }

        public long GetCurrentSegmentId(long segmentId)
        {
            return segmentId;
        }

        public long GetActiveSegmentId()
        {
            return long.Parse(_document.ActiveSegmentPair.Properties.Id.Id);
        }

        public string GetSegmentText(long segmentId)
        {
            return _segmentMetadata[segmentId];
        }
    }

  
}
