using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Text;
using Sdl.Community.ProjectTerms.Plugin.ExtractTerms;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsExtractor
    {
        private List<string> sourceTerms;
        private Dictionary<string, string> bilingualContentPair;
        public IFileTypeManager FileTypeManager { get; set; }

        public ProjectTermsExtractor()
        {
            sourceTerms = new List<string>();
            bilingualContentPair = new Dictionary<string, string>();
            FileTypeManager = DefaultFileTypeManager.CreateInstance(true);
        }

        public void ExtractBilingualContent(ProjectFile projectFile)
        {
            IMultiFileConverter converter = FileTypeManager.GetConverter(projectFile.LocalFilePath, (sender, e) => { });
            TextExtractionBilingualContentHandler extractor = new TextExtractionBilingualContentHandler();
            converter.AddBilingualProcessor(new Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.BilingualContentHandlerAdapter(extractor));
            converter.Parse();

            for (int i = 0; i < extractor.SourceText.Count; i++)
            {
                if (extractor.SourceText[i] == "" || extractor.TargetText[i] == "") continue;
                bilingualContentPair[extractor.SourceText[i].ToLower()] = extractor.TargetText[i].ToLower();
            }
        }

        public void ExtractProjectFileTerms(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            if (projectFile.Role != FileRole.Translatable) return;

            FileTypeManager.SettingsBundle = Core.Settings.SettingsUtil.CreateSettingsBundle(null);
            // disable xliff validation to speed up things
            FileTypeManager.SettingsBundle.GetSettingsGroup("SDL XLIFF 1.0 v 1.0.0.0").GetSetting<bool>("ValidateXliff").Value = false;

            TextExtractionBilingualSourceContentHandler extractor = new TextExtractionBilingualSourceContentHandler();
            multiFileConverter.AddBilingualProcessor(new Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.BilingualContentHandlerAdapter(extractor));
            multiFileConverter.Parse();

            foreach (var text in extractor.SourceText)
            {
                foreach (var term in GetTerms(text))
                {
                    sourceTerms.Add(term.ToLower());
                }
            }
        }

        protected virtual IEnumerable<string> GetTerms(string text)
        {
            StringBuilder term = new StringBuilder();
            bool containsLetter = false;
            List<string> terms = new List<string>();

            foreach (char ch in text)
            {
                if (char.IsLetter(ch))
                {
                    term.Append(ch);
                    containsLetter = true;
                }
                else if (char.IsDigit(ch))
                {
                    term.Append(ch);
                }
                else
                {
                    if (!term.Equals("") && containsLetter)
                    {
                        terms.Add(term.ToString());
                    }
                    term.Clear();
                    containsLetter = false;
                }
            }

            if (!term.ToString().Equals("")) terms.Add(term.ToString());

            return terms;
        }

        public List<string> GetSourceTerms()
        {
            return sourceTerms;
        }

        public Dictionary<string, string> GetBilingualContentPair()
        {
            return bilingualContentPair;
        }
    }
}
