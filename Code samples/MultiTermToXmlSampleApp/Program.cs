using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.MultiTerm.TMO.Interop;

namespace MultiTermToXmlSampleApp
{
    internal class Program
    {
        private static Termbase GetTermbase(Application multitermApplication, string termbasePath)
        {
            multitermApplication.LocalRepository.Termbases.Add(termbasePath, Path.GetFileName(termbasePath), string.Empty);
            var tb = multitermApplication.LocalRepository.Termbases.OfType<Termbase>().FirstOrDefault(t => t._Path.ToLower() == termbasePath.ToLower());
            return tb;
        }

        private static void ExportTermbaseToXml(Termbase termbase, MtTaskType exportType = MtTaskType.mtWizard)
        {
            // IMPORTANT: the first export definition is to xml
            var export = termbase.ExportDefinitions[0];
            export.ProcessExport(exportType);
        }

        private static void ExportTermbaseToXml(string termbasePath, MtTaskType exportType = MtTaskType.mtWizard)
        {
            var userId = string.Empty;
            var password = string.Empty;

            var multitermApplication = new Sdl.MultiTerm.TMO.Interop.Application();
            multitermApplication.LocalRepository.Connect(userId, password);

            var termbase = GetTermbase(multitermApplication, termbasePath);
            ExportTermbaseToXml(termbase, exportType);
        }

        static void Main(string[] args)
        {
            ExportTermbaseToXml("C:\\john\\OneDrive - SDL\\Documents\\Trados\\MultiTerm\\Termbases\\de pl multi.sdltb");
        }
    }
}
