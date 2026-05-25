using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Sdl.ProjectAutomation.FileBased;

namespace Multilingual.Excel.FileType.Services
{
    /// <summary>
    /// Rewrites legacy versioned file type identifiers
    /// (e.g. "Multilingual Excel FileType v 3.0.0.0") to the stable
    /// "Multilingual Excel FileType" identifier inside Trados Studio
    /// project files (.sdlproj) and bilingual files (.sdlxliff).
    /// Only the trailing " v X.X.X.X" suffix is removed; the rest of the file
    /// is preserved byte-for-byte (including original encoding/BOM and line endings).
    /// The operation is idempotent: files without legacy IDs are not modified.
    /// </summary>
    internal class FileTypeIdMigrationService
    {
        // Strict match: exactly four dot-separated integer components.
        // The first group ("Multilingual Excel FileType") is preserved by the
        // replacement; only the " v X.X.X.X" suffix is dropped.
        private const string LegacyIdPattern = @"(Multilingual Excel FileType) v \d+\.\d+\.\d+\.\d+";

        private static readonly Regex LegacyIdRegex = new Regex(
            LegacyIdPattern,
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        // Per-file dedup. A path is added only after the file was successfully
        // read AND either rewritten or confirmed to not need rewriting. Files
        // that fail (locked, IO error) are NOT recorded, so subsequent calls
        // (triggered by ProjectsChanged / CurrentProjectChanging) will retry.
        private readonly HashSet<string> _processedFilePaths =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Migrates the given project's .sdlproj and all .sdlxliff files under
        /// its folder. Safe to call repeatedly: files that already succeeded are
        /// skipped, files that previously failed (because they were locked) are
        /// retried.
        /// </summary>
        public void MigrateProject(FileBasedProject project)
        {
            if (project == null)
            {
                return;
            }

            string projectFilePath;
            try
            {
                projectFilePath = project.FilePath;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("MultilingualExcel: cannot read project FilePath: {0}", ex.Message);
                return;
            }

            if (string.IsNullOrEmpty(projectFilePath) || !File.Exists(projectFilePath))
            {
                return;
            }

            TryMigrateFile(projectFilePath);

            var projectFolder = Path.GetDirectoryName(projectFilePath);
            if (string.IsNullOrEmpty(projectFolder) || !Directory.Exists(projectFolder))
            {
                return;
            }

            IEnumerable<string> sdlxliffFiles;
            try
            {
                sdlxliffFiles = Directory.EnumerateFiles(projectFolder, "*.sdlxliff", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("MultilingualExcel: cannot enumerate sdlxliff files under '{0}': {1}", projectFolder, ex.Message);
                return;
            }

            foreach (var sdlxliffFile in sdlxliffFiles)
            {
                TryMigrateFile(sdlxliffFile);
            }
        }

        private void TryMigrateFile(string filePath)
        {
            if (_processedFilePaths.Contains(filePath))
            {
                return;
            }

            try
            {
                if (MigrateFile(filePath))
                {
                    _processedFilePaths.Add(filePath);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("MultilingualExcel: failed to migrate '{0}': {1}", filePath, ex.Message);
            }
        }

        /// <summary>
        /// Returns true when the file was processed deterministically (either rewritten
        /// or confirmed to not need rewriting). Returns false when the file could not
        /// be read/written due to a transient condition (e.g. Studio holds the file),
        /// so the caller can retry later.
        /// </summary>
        private static bool MigrateFile(string filePath)
        {
            byte[] originalBytes;
            try
            {
                // Use permissive sharing so we can read the file even when Studio
                // has it open (Studio typically opens with FileShare.Read).
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite | FileShare.Delete))
                {
                    originalBytes = new byte[fs.Length];
                    var offset = 0;
                    while (offset < originalBytes.Length)
                    {
                        var read = fs.Read(originalBytes, offset, originalBytes.Length - offset);
                        if (read <= 0)
                        {
                            break;
                        }
                        offset += read;
                    }
                }
            }
            catch (IOException ex)
            {
                // Likely in use exclusively by Studio; will retry later.
                Trace.TraceInformation("MultilingualExcel: cannot read '{0}' yet (will retry): {1}", filePath, ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.TraceInformation("MultilingualExcel: cannot read '{0}' yet (will retry): {1}", filePath, ex.Message);
                return false;
            }

            Encoding encoding;
            bool hasBom;
            DetectEncoding(originalBytes, out encoding, out hasBom);

            var preambleLength = hasBom ? encoding.GetPreamble().Length : 0;
            var original = encoding.GetString(originalBytes, preambleLength, originalBytes.Length - preambleLength);

            if (!LegacyIdRegex.IsMatch(original))
            {
                // Nothing to do; mark as processed so we don't keep re-reading it.
                return true;
            }

            // "$1" keeps the captured "Multilingual Excel FileType" and drops the
            // " v X.X.X.X" suffix. Nothing else in the file is touched.
            var updated = LegacyIdRegex.Replace(original, "$1");
            if (string.Equals(updated, original, StringComparison.Ordinal))
            {
                return true;
            }

            byte[] updatedBytes;
            if (hasBom)
            {
                var preamble = encoding.GetPreamble();
                var contentBytes = encoding.GetBytes(updated);
                updatedBytes = new byte[preamble.Length + contentBytes.Length];
                Buffer.BlockCopy(preamble, 0, updatedBytes, 0, preamble.Length);
                Buffer.BlockCopy(contentBytes, 0, updatedBytes, preamble.Length, contentBytes.Length);
            }
            else
            {
                updatedBytes = encoding.GetBytes(updated);
            }

            var tempPath = filePath + ".mlxmig.tmp";
            try
            {
                File.WriteAllBytes(tempPath, updatedBytes);

                // Atomic replace so a crash mid-write cannot truncate the original.
                // This requires exclusive write access to filePath. If Studio holds
                // an exclusive lock, this will throw and we return false so the
                // caller can retry later (e.g. when the project is closed).
                File.Replace(tempPath, filePath, null, ignoreMetadataErrors: true);
                Trace.TraceInformation("MultilingualExcel: migrated legacy file type id in '{0}'.", filePath);
                return true;
            }
            catch (IOException ex)
            {
                Trace.TraceInformation("MultilingualExcel: cannot rewrite '{0}' yet (will retry): {1}", filePath, ex.Message);
                return false;
            }
            catch (UnauthorizedAccessException ex)
            {
                Trace.TraceInformation("MultilingualExcel: cannot rewrite '{0}' yet (will retry): {1}", filePath, ex.Message);
                return false;
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    try { File.Delete(tempPath); } catch { /* ignored */ }
                }
            }
        }

        private static void DetectEncoding(byte[] bytes, out Encoding encoding, out bool hasBom)
        {
            // UTF-8 BOM: EF BB BF
            if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            {
                encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
                hasBom = true;
                return;
            }

            // UTF-16 LE BOM: FF FE
            if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                encoding = new UnicodeEncoding(bigEndian: false, byteOrderMark: true);
                hasBom = true;
                return;
            }

            // UTF-16 BE BOM: FE FF
            if (bytes.Length >= 2 && bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                encoding = new UnicodeEncoding(bigEndian: true, byteOrderMark: true);
                hasBom = true;
                return;
            }

            // No BOM: assume UTF-8 (Studio's XML files are UTF-8 by default).
            encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            hasBom = false;
        }
    }
}
