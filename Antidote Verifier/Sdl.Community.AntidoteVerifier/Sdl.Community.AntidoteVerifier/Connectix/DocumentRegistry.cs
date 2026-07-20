using System;
using System.Collections.Generic;
using NLog;

namespace Sdl.Community.AntidoteVerifier.Connectix
{
    /// <summary>
    /// Connection-lifetime registry of every document launched over the Connectix connection, and
    /// owner of the zone-id scheme that makes zone-id-bearing callbacks routable. Each document gets
    /// a short stable key (<c>"d1"</c>, <c>"d2"</c>, ...) keyed by its path — the same identity
    /// Antidote uses for the correction tab — so a relaunch reuses the key and callbacks issued
    /// against zones from an earlier launch still resolve to the right document. Zone ids are
    /// <c>"{documentKey}:{segmentIndex}"</c>; a bare integer (an id issued before this scheme, e.g.
    /// restored from Antidote's persisted correction state) falls back to the active document.
    /// Thread-safe: the launcher registers on a worker thread while the transport thread resolves.
    /// </summary>
    internal sealed class DocumentRegistry
    {
        private const char ZoneIdSeparator = ':';

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly object _gate = new object();
        private readonly Dictionary<string, DocumentEntry> _documentsByPath =
            new Dictionary<string, DocumentEntry>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, DocumentEntry> _documentsByKey =
            new Dictionary<string, DocumentEntry>(StringComparer.Ordinal);
        private int _nextDocumentNumber = 1;

        // The document of the most recent launch: identifier-less callbacks are answered for it.
        // Volatile because the transport thread reads it while the launcher swaps it per launch.
        private volatile DocumentEntry _active;

        /// <summary>The entry of the most recently launched document.</summary>
        public DocumentEntry Active => _active;

        /// <summary>
        /// Registers <paramref name="editor"/> as the latest editor for the document at
        /// <paramref name="documentPath"/> (creating the entry on first launch) and makes that
        /// document the active one.
        /// </summary>
        public DocumentEntry Register(IEditorService editor, string documentPath)
        {
            var path = documentPath ?? string.Empty;

            lock (_gate)
            {
                if (!_documentsByPath.TryGetValue(path, out var entry))
                {
                    entry = new DocumentEntry("d" + _nextDocumentNumber++, ZoneIdSeparator);
                    _documentsByPath.Add(path, entry);
                    _documentsByKey.Add(entry.Key, entry);
                }

                entry.Editor = editor;
                _active = entry;
                return entry;
            }
        }

        /// <summary>
        /// Resolves a zone id to the editor of the document it was issued for. Unknown document keys
        /// fail the resolution so the callback becomes a safe no-op instead of editing the wrong
        /// document.
        /// </summary>
        public bool TryResolveZone(string zoneId, out IEditorService editor, out int segmentIndex)
        {
            editor = null;
            segmentIndex = 0;
            if (string.IsNullOrEmpty(zoneId))
                return false;

            var separator = zoneId.LastIndexOf(ZoneIdSeparator);
            if (separator < 0)
            {
                if (!int.TryParse(zoneId, out segmentIndex))
                    return false;
                editor = _active.Editor;
                return true;
            }

            if (!int.TryParse(zoneId.Substring(separator + 1), out segmentIndex))
                return false;

            var key = zoneId.Substring(0, separator);
            DocumentEntry entry;
            lock (_gate)
            {
                if (!_documentsByKey.TryGetValue(key, out entry))
                {
                    Logger.Warn("Antidote callback references unknown document key '{0}' (zoneId '{1}'); ignoring.",
                        key, zoneId);
                    return false;
                }
            }

            editor = entry.Editor;
            return true;
        }

        /// <summary>
        /// One launched document: its zone-id prefix and the latest editor bound to it. The editor
        /// field is volatile because the transport thread routes callbacks to it while the launcher
        /// swaps it (Studio news an <see cref="EditorService"/> up per launch).
        /// </summary>
        public sealed class DocumentEntry
        {
            private readonly char _zoneIdSeparator;

            public DocumentEntry(string key, char zoneIdSeparator)
            {
                Key = key;
                _zoneIdSeparator = zoneIdSeparator;
            }

            public string Key { get; }
            public volatile IEditorService Editor;

            public string MakeZoneId(int segmentIndex) => Key + _zoneIdSeparator + segmentIndex;
        }
    }
}
