using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

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

        private ConceptGrpLanguageGrp CreateLanguageGroup(string text, Language lang)
        {
            var termGrp = new ConceptGrpLanguageGrpTermGrp();
            termGrp.Term = text;

            var language = new ConceptGrpLanguageGrpLanguage();
            language.Lang = lang.DisplayName.ToUpper();
            language.Type = lang.IsoAbbreviation.ToUpper();


            var languageGrp = new ConceptGrpLanguageGrp();
            languageGrp.Language = language;
            languageGrp.TermGrp = termGrp;

            return languageGrp;
        }

        public static string Serialize(ConceptGrp dataToSerialize)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(ConceptGrp));
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
                settings.Indent = true;
                settings.OmitXmlDeclaration = true;

                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
            catch
            {
                throw;
            }
        }

        private string CreateEntry(string sourceText, Language sourceLang, string targetText, Language targetLang)
        {
            var conceptGrp = new ConceptGrp();
            conceptGrp.Concept = sbyte.Parse("-1");

            ConceptGrpLanguageGrp[] languageGrp = new ConceptGrpLanguageGrp[] {
                CreateLanguageGroup(sourceText, sourceLang),
                CreateLanguageGroup(targetText, targetLang)
            };

            conceptGrp.LanguageGrp = languageGrp;
            string result = Serialize(conceptGrp);
            return result;
            
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

            Entries oEntries = oTb.Entries;
            foreach (var item in bilingualContentPair.Keys)
            {
                string entry = CreateEntry(item, selectedFile.SourceFile.Language, bilingualContentPair[item], selectedFile.Language);

                //oEntries.New(entry, true);
            }
        }
    }
}
