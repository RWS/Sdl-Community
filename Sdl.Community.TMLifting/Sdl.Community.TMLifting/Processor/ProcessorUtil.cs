using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;

namespace Sdl.Community.TMLifting.Processor
{
    public class ProcessorUtil
    {
        public static string GetOutputTmFullPath(string name,string filePath)
        {
            var fullPath = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal));
            var tmName = name.Substring(0, name.LastIndexOf(".", StringComparison.Ordinal));
            var extension = Path.GetExtension(filePath);
            return Path.Combine(fullPath, tmName + extension);
        }

        public static string RenameOriginalTm(string name, string filePath)
        {
            var fullPath = filePath.Substring(0, filePath.LastIndexOf(@"\", StringComparison.Ordinal));
            var extension = Path.GetExtension(filePath);
            var tmName = name.Substring(0, name.LastIndexOf(".", StringComparison.Ordinal));
            var renamedTm = Path.Combine(fullPath, tmName + "_original" + extension);
            if (File.Exists(renamedTm))
            {
                File.Delete(renamedTm);
            }
            File.Move(filePath, renamedTm);
            return renamedTm;

        }

        public static void UpdateTranslationMemory(FileBasedTranslationMemory tm)
        {
            if (tm.FGASupport == FGASupport.NonAutomatic)
                return;

            tm.Save();
        }
    }
}