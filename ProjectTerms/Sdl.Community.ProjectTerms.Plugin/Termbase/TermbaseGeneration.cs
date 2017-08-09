using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.Termbase
{
    public class TermbaseGeneration : AbstractBilingualContentHandler
    {
        /// <summary>
        /// Create a termbase and add it to Termbases file in sdl project.
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        /// <returns></returns>
        public ITermbase CreateTermbase(string termbaseDefinitionPath)
        {
            // Prepare the thermbase local repository
            MultiTerm.TMO.Interop.Application multiTermClientObject = new MultiTerm.TMO.Interop.Application();
            TermbaseRepository localRepository = multiTermClientObject.LocalRepository;
            localRepository.Connect("", "");
            Termbases oTbs = localRepository.Termbases;

            ProjectInfo sdlProjectInfo = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo();
            string termbasePath = Path.Combine(sdlProjectInfo.LocalProjectFolder + "\\Termbases" + "\\" + sdlProjectInfo.Name, "projectTerms.sdltb");
            if (!Directory.Exists(Path.GetDirectoryName(termbasePath))) Directory.CreateDirectory(Path.GetDirectoryName(termbasePath));
            if (File.Exists(termbasePath)) MessageBox.Show("Termbase exists.");
            ITermbase oTb = oTbs.New("projectTermsTermbase", "Optional Description", termbaseDefinitionPath, termbasePath);

            return oTb;
        }

        /// <summary>
        /// Extract the project languages to include them in termbase
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectLanguages()
        {
            var langs = new Dictionary<string, string>();
            Language sourceLanguage = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().SourceLanguage;
            langs[sourceLanguage.DisplayName] = sourceLanguage.IsoAbbreviation.ToUpper();
            var targetLanguages = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().TargetLanguages;
            foreach (var lang in targetLanguages)
            {
                langs[lang.DisplayName] = lang.IsoAbbreviation.ToUpper();
            }

            return langs;
        }

        private string CreateEntry(string sourceText, Language sourceLang, string targetText, Language targetLang)
        {
            return new XElement("conceptGrp",
                    new XElement("languageGrp", 
                        new XElement("language", new XAttribute("lang", sourceLang.IsoAbbreviation.ToUpper()), new XAttribute("type", sourceLang.DisplayName.ToUpper())),
                        new XElement("termGrp", new XElement("term", sourceText))),
                    new XElement("languageGrp",
                        new XElement("language", new XAttribute("lang", targetLang.IsoAbbreviation.ToUpper()), new XAttribute("type", targetLang.DisplayName.ToUpper())),
                        new XElement("termGrp", new XElement("term", targetText)))
                ).ToString();
        }

        /// <summary>
        /// Add entries to a given termbase
        /// </summary>
        /// <param name="oTb"></param>
        public void PopulateTermbase(ITermbase oTb)
        {
            ProjectTermsExtractor extractor = new ProjectTermsExtractor();
            var selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();

            extractor.ExtractBilingualContent(selectedFile);

            Dictionary<string, string> bilingualContentPair = extractor.GetBilingualContentPair();
            foreach (var item in bilingualContentPair.Keys)
            {
                string entry = CreateEntry(item, selectedFile.SourceFile.Language, bilingualContentPair[item], selectedFile.Language);
                Entries oEntries = oTb.Entries;
                oEntries.New(entry, true);
            }
        }
    }
}
