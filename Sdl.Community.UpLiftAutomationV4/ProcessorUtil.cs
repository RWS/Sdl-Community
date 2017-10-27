using Sdl.Core.FineGrainedAlignment;
using Sdl.Desktop.Common;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;
using System.Text;

namespace Sdl.Community.FragmentAlignmentAutomation
{
    internal class ProcessorUtil
    {
        public static void UpdateTranslationMemory(FileBasedTranslationMemory tm)
        {
            if (tm.FGASupport == FGASupport.Off)
                return;

            tm.Save();
        }

        public static void ShowException(Exception ex)
        {
            if (ex == null)
                return;

            using (var d = new ExceptionDialog())
            {
                d.Message = ex.Message;
                d.Exception = ex;
                d.ShowErrorDetails = true;
                d.ShowDialog();
            }
        }

        public static string GetProgresssStageText(TranslationModelProgressStage progresssStage)
        {
            switch (progresssStage)
            {
                case TranslationModelProgressStage.Preparing:
                    return StringResources.FragmentAlignment_ProgressPreparingMessage;
                case TranslationModelProgressStage.Encoding:
                    return StringResources.FragmentAlignment_ProgressEncodingMessage;
                case TranslationModelProgressStage.Computing:
                    return StringResources.FragmentAlignment_ProgressComputingMessage;
                default:
                    return StringResources.FragmentAlignment_ProgressPreparingMessage;
            }
        }

        public static string ExceptionToMsg(Exception e)
        {
            var sb = new StringBuilder();
            while (e != null)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(e.Message);
                e = e.InnerException;
            }
            return sb.ToString();
        }

        public static int GetPercentage(int total, int current)
        {
            if (total >= current && total > 0)
                return (int)Math.Round((Convert.ToDouble(current) / Convert.ToDouble(total)) * 100, 0);
            return 0;
        }

        public static string GetProgressMessage(ProgressEventArgs e)
        {
            return string.Format(
                "{0}{1}"
                , e.Type
                , string.IsNullOrEmpty(e.Description) ? string.Empty : " (" + e.Description + ")");
        }

        public static string GetOutputTmFullPath(FileInfo fileInfo)
        {
            var name = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            return Path.Combine(fileInfo.DirectoryName, name + "_FAS" + fileInfo.Extension);
        }

    }
}
