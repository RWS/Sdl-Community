using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.ReindexTms.Processors.Extensions;
using Sdl.Community.ReindexTms.Properties;
using Sdl.Core.FineGrainedAlignment;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ReindexTms.Processors
{
    class ProcessorUtil
    {
        public static void UpdateTranslationMemory(FileBasedTranslationMemory tm)
        {
            //if (tm.FGASupport == FGASupport.None)
            //    return;
            if (tm.FGASupport == FGASupport.NonAutomatic)
                return;

            tm.Save();
        }

        public static void ShowException(Exception ex)
        {
            if (ex == null)
                return;

            //using (var d = new ExceptionDialog())
            //{
            //    d.Message = ex.Message;
            //    d.Exception = ex;
            //    d.ShowErrorDetails = true;
            //    d.ShowDialog();
            //}
        }

        public static string GetProgresssStageText(TranslationModelProgressStage progresssStage)
        {
            switch (progresssStage)
            {
                case TranslationModelProgressStage.Preparing:
                    return Resources.FragmentAlignment_ProgressPreparingMessage;
                case TranslationModelProgressStage.Encoding:
                    return Resources.FragmentAlignment_ProgressEncodingMessage;
                case TranslationModelProgressStage.Computing:
                    return Resources.FragmentAlignment_ProgressComputingMessage;
                default:
                    return Resources.FragmentAlignment_ProgressPreparingMessage;
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
