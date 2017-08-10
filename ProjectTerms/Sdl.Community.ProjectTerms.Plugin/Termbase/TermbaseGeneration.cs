using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.Termbase
{
    public class TermbaseGeneration : AbstractBilingualContentHandler
    {
        private string termbasePath;
        private FileBasedProject project;
        private Dictionary<string, string> langs;

        public TermbaseGeneration()
        {
            langs = new Dictionary<string, string>();
        }

        private void CleanLocalTempDirectory(string termbaseDefinitionPath)
        {
            string termbaseDefinitionDirectory = Path.GetDirectoryName(termbaseDefinitionPath);
            File.Delete(termbaseDefinitionPath);
            Directory.Delete(termbaseDefinitionDirectory);
        }

        private void SetDataMember()
        {
            project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;

            var myDocumentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //TO DO:
            //var studioVersionService = new Toolkit.Core.Services.StudioVersionService();
            //var studioVersion = studioVersionService.GetStudioVersion().Version;
            var selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();
            termbasePath = Path.Combine(myDocumentsPath + "\\Studio 2017" + "\\Termbases", Path.GetFileNameWithoutExtension(selectedFile.LocalFilePath) + ".sdltb");
        }

        private Termbases ConnectToTermbaseLocalRepository()
        {
            MultiTerm.TMO.Interop.Application multiTermClientObject = new MultiTerm.TMO.Interop.Application();
            TermbaseRepository localRepository = multiTermClientObject.LocalRepository;
            localRepository.Connect("", "");
            return localRepository.Termbases;
        }

        /// <summary>
        /// Create a termbase and add it to Termbases file in sdl project.
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        /// <returns></returns>
        public ITermbase CreateTermbase(string termbaseDefinitionPath)
        {
            Termbases termbases = ConnectToTermbaseLocalRepository();

            SetDataMember();
            if (File.Exists(termbasePath)) return null;

            ITermbase termbase = termbases.New("projectTermsTermbase", "Optional Description", termbaseDefinitionPath, termbasePath);

            CleanLocalTempDirectory(termbaseDefinitionPath);
            return termbase;
        }

        /// <summary>
        /// Extract the project languages to include them in termbase
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectLanguages()
        {
            Language sourceLanguage = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().SourceLanguage;
            langs[sourceLanguage.DisplayName] = sourceLanguage.IsoAbbreviation.ToUpper();
            var targetLanguages = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject.GetProjectInfo().TargetLanguages;
            foreach (var lang in targetLanguages)
            {
                langs[lang.DisplayName] = lang.IsoAbbreviation.ToUpper();
            }

            return langs;
        }

        private string CreateEntry(string sourceText, Language sourceLang, List<KeyValuePair<string, string>> targets)
        {
            return new XElement("conceptGrp",
                // Add source text
                new XElement("languageGrp",
                    new XElement("language", new XAttribute("lang", sourceLang.IsoAbbreviation.ToUpper()), new XAttribute("type", sourceLang.DisplayName.ToUpper())),
                    new XElement("termGrp", new XElement("term", sourceText))),
                // Add target texts
                targets.Select(item => 
                     new XElement("languageGrp",
                        new XElement("language", new XAttribute("lang", langs[item.Value]), new XAttribute("type", item.Value)),
                        new XElement("termGrp", new XElement("term", item.Key)))
                )
            ).ToString();
        }

        /// <summary>
        /// Add entries to a given termbase
        /// </summary>
        /// <param name="oTb"></param>
        public void PopulateTermbase(ITermbase termbase)
        {
            ProjectTermsExtractor extractor = new ProjectTermsExtractor();

            var targetProjectFiles = project.GetTargetLanguageFiles();
            var selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();
            var targetFilesReportedToSelectedFile = targetProjectFiles.Where(file => file.Name.Equals(selectedFile.Name));

            extractor.ExtractBilingualContent(targetFilesReportedToSelectedFile.ToArray());

            Dictionary<string, List<KeyValuePair<string, string>>> bilingualContentPair = extractor.GetBilingualContentPair();
            foreach (var item in bilingualContentPair.Keys)
            {
                string entry = CreateEntry(item, selectedFile.SourceFile.Language, bilingualContentPair[item]);
                Entries oEntries = termbase.Entries;
                oEntries.New(entry, true);
            }
        }
    }
}
