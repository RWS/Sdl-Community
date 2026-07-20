using System;
using System.Collections.Generic;
using Sdl.Community.AntidoteVerifier;

namespace Sdl.Community.AntidoteVerifier.Tests.Fakes
{
    /// <summary>
    /// In-memory <see cref="IEditorService"/> backed by a 1-based list of segment strings, mirroring how
    /// <c>EditorService</c> exposes correctable segments. Replacement mutates the stored text so tests can
    /// assert the final document state. Lock behaviour is configurable to exercise the <c>CanReplace</c> gate.
    /// </summary>
    public sealed class FakeEditorService : IEditorService
    {
        private readonly List<string> _segments;

        public FakeEditorService(params string[] segments)
        {
            _segments = new List<string>(segments ?? Array.Empty<string>());
        }

        public int ActiveSegmentId { get; set; } = 1;
        public string DocumentName { get; set; } = "FakeDocument";
        public string DocumentPath { get; set; } = @"C:\docs\FakeDocument.sdlxliff";
        public bool CanReplaceResult { get; set; } = true;

        public int SelectCalls { get; private set; }
        public int ActivateCalls { get; private set; }
        public (int Index, int Start, int End)? LastSelect { get; private set; }

        public IReadOnlyList<string> Segments => _segments;

        public int GetDocumentNoOfSegments() => _segments.Count;

        public int GetActiveSegmentId() => ActiveSegmentId;

        public string GetSegmentText(int index)
        {
            if (index < 1 || index > _segments.Count) return null;
            return _segments[index - 1];
        }

        public bool CanReplace(int index, int startPosition, int endPosition, string origString,
            string displayLanguage, ref string message, ref string explication) => CanReplaceResult;

        public string GetDocumentName() => DocumentName;

        public string GetDocumentPath() => DocumentPath;

        public void ReplaceTextInSegment(int index, int startPosition, int endPosition, string segmentText)
        {
            var current = _segments[index - 1];
            _segments[index - 1] = current.Substring(0, startPosition)
                + segmentText
                + current.Substring(endPosition);
        }

        public void SelectText(int index, int startPosition, int endPosition)
        {
            SelectCalls++;
            LastSelect = (index, startPosition, endPosition);
        }

        public void ActivateDocument() => ActivateCalls++;
    }
}
