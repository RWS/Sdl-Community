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
        private Dictionary<int, KeyValuePair<int,string>> _segmentMetadata;
        
        public EditorService(Document document)
        {
            _document = document;
            _segmentMetadata = new Dictionary<int, KeyValuePair<int,string>>();
            Initialize();
        }

        private void Initialize()
        {
            _segmentMetadata.Clear();
            var activeSegmentId = int.Parse(_document.ActiveSegmentPair.Properties.Id.Id);
            int index = 1;
            foreach (var segmentPair in _document.SegmentPairs)
            {
                var paragraphUnitId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;
                if (string.IsNullOrEmpty(segmentPair.Target.GetString())) continue;
                var currentId = int.Parse(segmentPair.Properties.Id.Id);
                if (currentId < activeSegmentId) continue;
                _segmentMetadata.Add(index, new KeyValuePair<int, string>(currentId, paragraphUnitId));
                index++;
            }
        }

        private ISegmentPair GetSegmentPair(int index)
        {
            var segmentUniqueIdentifier = _segmentMetadata[index];

            return _document.SegmentPairs
                .FirstOrDefault(
                        segmentPair =>
                        {
                            var segmentIdFound = segmentPair.Properties.Id.Id.Equals(segmentUniqueIdentifier.Key.ToString());
                            var paragraphUnitId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;
                            return segmentIdFound && paragraphUnitId.Equals(segmentUniqueIdentifier.Value);
                        });
        }
        public int GetDocumentId()
        {
           return DocumentIdGenerator.Instance.GetDocumentId(_document.ActiveFile.Id);
        }
       
        public int GetDocumentNoOfSegments()
        {
           
            return _segmentMetadata.Count();
        }

        public int GetCurrentSegmentId(int segmentNumber)
        {
            return segmentNumber;
        }

        public int GetActiveSegmentId()
        {
            var segmentId = int.Parse(_document.ActiveSegmentPair.Properties.Id.Id);
            var paragraphUnitId = _document.ActiveSegmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;
            foreach (var kvp in _segmentMetadata)
            {
                if(kvp.Value.Key.Equals(segmentId) && kvp.Value.Value.Equals(paragraphUnitId))
                {
                    return kvp.Key;
                }
            }

            return 1;
        }

        public string GetSegmentText(int index)
        {
            var segmentPair = GetSegmentPair(index);


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

        public void SelectText(int index, int startPosition, int endPosition)
        {
            var segmentPair = GetSegmentPair(index);

            var paragraphUnitId = segmentPair.GetParagraphUnitProperties().ParagraphUnitId.Id;

            _document.SetActiveSegmentPair(paragraphUnitId, segmentPair.Properties.Id.Id);
        }

        public void ActivateDocument()
        {
            
            //Commented because Activate is not thread-safe and it will crash.
            //EditorController editorController = SdlTradosStudio.Application.GetController<EditorController>();
            //editorController.Activate(_document);
        }
    }

  
}
