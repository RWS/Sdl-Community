using System.Collections.Generic;
using Sdl.Community.AntidoteVerifier.Connectix.Protocol;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Application-side seam Antidote queries during a correction session. This is the C# analog of
    /// Druide's <c>WordProcessorAgent</c>: it answers the protocol callbacks while hiding the Studio
    /// editor behind <see cref="IEditorService"/>. Kept narrow to the messages this plugin uses
    /// (Corrector, Dictionaries, Guides); the dispatcher no-ops the unused ones.
    /// </summary>
    public interface IWordProcessorAgent
    {
        WordProcessorConfiguration Configuration();

        string DocumentPath();

        bool IsDocumentAvailable();

        /// <param name="forActiveSelection">
        /// When true Antidote wants a single zone holding the current selection (dictionaries/guides);
        /// when false it wants every correctable zone in the document (corrector).
        /// </param>
        IReadOnlyList<TextZone> ZonesToCorrect(bool forActiveSelection);

        bool AllowEdit(AllowEditParams parameters);

        bool Replace(ReplaceParams parameters);

        void Select(SelectParams parameters);

        void ReturnToDocument();
    }
}
