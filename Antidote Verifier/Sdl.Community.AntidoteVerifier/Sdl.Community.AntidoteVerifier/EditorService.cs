using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AntidoteVerifier.Utils;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using System.Reflection;
using Sdl.DesktopEditor.EditorApi;

namespace Sdl.Community.AntidoteVerifier
{
    public class EditorService : IEditorService
    {
        private Document _document;
        private Dictionary<int, int> _segmentMetadata;
        
        public EditorService(Document document)
        {
            _document = document;
            _segmentMetadata = new Dictionary<int, int>();
        }

        private ISegmentPair GetSegmentPair(int segmentId)
        {
            return _document.SegmentPairs
                .FirstOrDefault(x => x.Properties.Id.Id.Equals(segmentId.ToString()));
        }
        public int GetDocumentId()
        {
           return DocumentIdGenerator.Instance.GetDocumentId(_document.ActiveFile.Id);
        }
       
        public int GetDocumentNoOfSegments()
        {
            _segmentMetadata.Clear();
            var activeSegmentId = GetActiveSegmentId();
            int index = 1;
            foreach (var segmentPair in _document.SegmentPairs)
            {
                if (string.IsNullOrEmpty(segmentPair.Target.GetString())) continue;
                var currentId = int.Parse(segmentPair.Properties.Id.Id);
                if (currentId < activeSegmentId) continue;
                _segmentMetadata.Add(index, currentId);
                index++;
            }
            return _segmentMetadata.Count();
        }

        public int GetCurrentSegmentId(int segmentNumber)
        {
            return _segmentMetadata[segmentNumber];
        }

        public int GetActiveSegmentId()
        {
            return int.Parse(_document.ActiveSegmentPair.Properties.Id.Id);
        }

        public string GetSegmentText(int segmentId)
        {
            var segmentPair = GetSegmentPair(segmentId);


            return segmentPair.Target.GetString();
        }

        public string GetSelection()
        {
            return _document.Selection.Target.ToString();
        }

        public string GetDocumentName()
        {
            return _document.ActiveFile.Name;
        }

        public void ReplaceTextInSegment(int segmentId, int startPosition, int endPosition, string replacementText)
        {
            var segmentPair = GetSegmentPair(segmentId);

            segmentPair.Target.Replace(startPosition, endPosition, replacementText);
           
            _document.UpdateSegmentPair(segmentPair);

        }

        public void SelectText(int segmentId, int startPosition, int endPosition)
        {
            var segmentPair = GetSegmentPair(segmentId);

            var paragraphUnitId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;

            _document.SetActiveSegmentPair(paragraphUnitId, segmentId.ToString());
            
        }

        public void ActivateDocument()
        {
            
            //Commented because Activate is not thread-safe and it will crash.
            //EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            //editorController.Activate(_document);
        }
    }

  
}
