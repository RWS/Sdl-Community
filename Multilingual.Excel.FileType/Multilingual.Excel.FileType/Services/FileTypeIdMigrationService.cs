using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Multilingual.Excel.FileType.Constants;

namespace Multilingual.Excel.FileType.Services
{
    /// <summary>
    /// Rewrites Multilingual Excel FileType identifiers found inside Trados Studio
    /// project files (.sdlproj) and bilingual files (.sdlxliff) so that they match
    /// the currently installed plugin's versioned identifier
    /// (e.g. "Multilingual Excel FileType v 3.0.2.0").
    /// Both legacy versioned ids ("Multilingual Excel FileType v 1.0.0.0") and the
    /// unversioned form ("Multilingual Excel FileType") are upgraded to the current
    /// versioned id. The rest of the file is preserved byte-for-byte (including
    /// original encoding/BOM and line endings).
    /// The operation is idempotent: files already on the current id are not modified.
    /// </summary>
    internal class FileTypeIdMigrationService
    {
        // Matches the file type id prefix with an optional " v X.X.X.X" version suffix.
        // The whole match is replaced with the current plugin's versioned id, so files
        // that contain a stale version (or no version at all) get upgraded.
        private const string FileTypeIdPattern = @"Multilingual Excel FileType( v \d+\.\d+\.\d+\.\d+)?";

        private static readonly Regex FileTypeIdRegex = new Regex(
            FileTypeIdPattern,
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly string CurrentFileTypeId = FiletypeConstants.FileTypeDefinitionId;

        // Per-file dedup. A path is added only after the file was successfully
        // read AND either rewritten or confirmed to not need rewriting. Files
        // that fail (locked, IO error) are NOT recorded, so subsequent calls
        // (triggered by ProjectsChanged / CurrentProjectChanging) will retry.
        private readonly HashSet<string> _processedFilePaths =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Migrates the .sdlproj at <paramref name="projectFilePath"/> and all .sdlxliff
        /// files under its folder. Safe to call repeatedly: files that already succeeded
        /// are skipped, files that previously failed (because they were locked) are
        /// retried. The caller is responsible for closing the project in Studio before
        /// calling this method.
        /// </summary>
        public void MigrateProject(string projectFilePath)
        {
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

            if (!FileTypeIdRegex.IsMatch(original))
            {
                // Nothing to do; mark as processed so we don't keep re-reading it.
                return true;
            }

            // Replace every occurrence (legacy versioned or unversioned) with the
            // current plugin's versioned id. Idempotent: if every occurrence already
            // matches CurrentFileTypeId, the resulting string is identical.
            var updated = FileTypeIdRegex.Replace(original, CurrentFileTypeId);
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
                Trace.TraceInformation("MultilingualExcel: rewrote file type id to '{0}' in '{1}'.", CurrentFileTypeId, filePath);
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
