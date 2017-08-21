using Sdl.Community.ProjectTerms.Plugin.Exceptions;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.MultiTerm.TMO.Interop;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Sdl.Community.ProjectTerms.Plugin.TermbaseIntegrationAction
{
    public class TermbaseGeneration : AbstractBilingualContentHandler
    {
        private FileBasedProject project;
        private ProjectFile selectedFile;
        private string termbasePath;
        private Dictionary<string, string> langs;

        public TermbaseGeneration() { langs = new Dictionary<string, string>(); }

        private void CheckedTermbaseDirectoryExists(string termbasePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(termbasePath)))
            {
                Utils.Utils.CreateDirectory(Path.GetDirectoryName(termbasePath));
            }
        }

        private void Settings()
        {
            try
            {
                project = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
                selectedFile = SdlTradosStudio.Application.GetController<FilesController>().SelectedFiles.FirstOrDefault();

                termbasePath = Path.Combine(Path.GetDirectoryName(project.FilePath) + "\\Tb", Path.GetFileNameWithoutExtension(selectedFile.LocalFilePath) + ".sdltb");

                CheckedTermbaseDirectoryExists(termbasePath);
            }
            catch (Exception e)
            {
                throw new TermbaseGenerationException(PluginResources.Error_Settings + e.Message);
            }
        }

        /// <summary>
        /// Connect to local termbase repository
        /// </summary>
        /// <returns></returns>
        private Termbases ConnectToTermbaseLocalRepository()
        {
            try
            {
                var multiTermClientObject = new Application();
                var localRepository = multiTermClientObject.LocalRepository;
                localRepository.Connect("", "");
                return localRepository.Termbases;
            } catch(Exception e)
            {
                throw new TermbaseGenerationException(PluginResources.Error_ConnectToTermbaseLocalRepository + e.Message);
            }
        }

        /// <summary>
        /// Create a termbase and add it to Termbases file in sdl project.
        /// </summary>
        /// <param name="termbaseDefinitionPath"></param>
        /// <returns></returns>
        public ITermbase CreateTermbase(string termbaseDefinitionPath)
        {
            try
            {
                var termbases = ConnectToTermbaseLocalRepository();

                if (File.Exists(termbasePath)) return null;

                var termbase = termbases.New(Path.GetFileNameWithoutExtension(selectedFile.LocalFilePath), "Optional Description", termbaseDefinitionPath, termbasePath);

                Utils.Utils.RemoveDirectory(Path.GetDirectoryName(termbaseDefinitionPath));
                return termbase;
            } catch (Exception e)
            {
                throw new TermbaseGenerationException(PluginResources.Error_CreateTermbase + e.Message);
            }
        }

        /// <summary>
        /// Create xml entry as string
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="sourceLang"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        private string CreateEntry(string sourceText, Language sourceLang, List<KeyValuePair<string, string>> targets)
        {
            try
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
            } catch(Exception e)
            {
                throw new TermbaseGenerationException(PluginResources.Error_CreateEntry + e.Message);
            }
        }

        /// <summary>
        /// Add entries to a given termbase
        /// </summary>
        /// <param name="oTb"></param>
        public void PopulateTermbase(ITermbase termbase)
        {
            try
            {
                var extractor = new ProjectTermsExtractor();

                var targetProjectFiles = project.GetTargetLanguageFiles();
                var targetFilesReportedToSelectedFile = targetProjectFiles.Where(file => file.Name.Equals(selectedFile.Name));

                extractor.ExtractBilingualContent(targetFilesReportedToSelectedFile.ToArray());

                Dictionary<string, List<KeyValuePair<string, string>>> bilingualContentPair = extractor.GetBilingualContentPair();
                foreach (var item in bilingualContentPair.Keys)
                {
                    var entry = CreateEntry(item, selectedFile.SourceFile.Language, bilingualContentPair[item]);
                    var oEntries = termbase.Entries;
                    oEntries.New(entry, true);
                }
            }
            catch (Exception e)
            {
                throw new TermbaseGenerationException(PluginResources.Error_PopulateTermbase + e.Message);
            }
        }

        /// <summary>
        /// Extract the project languages to include them in termbase
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetProjectLanguages()
        {
            try
            {
                if (project == null) Settings();

                var sourceLanguage = project.GetProjectInfo().SourceLanguage;
                langs[sourceLanguage.DisplayName] = sourceLanguage.IsoAbbreviation.ToUpper();
                var targetLanguages = project.GetProjectInfo().TargetLanguages;
                foreach (var lang in targetLanguages)
                {
                    langs[lang.DisplayName] = lang.IsoAbbreviation.ToUpper();
                }

                return langs;
            }
            catch (Exception e)
            {
                throw new TermbaseGenerationException(PluginResources.Error_GetProjectLanguages + e.Message);
            }
        }
    }
}
