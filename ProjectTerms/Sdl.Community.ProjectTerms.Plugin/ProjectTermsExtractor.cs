using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using System.Collections.Generic;
using System.Text;

namespace Sdl.Community.ProjectTerms.Plugin
{
    public class ProjectTermsExtractor
    {
        private List<string> terms;
        public IFileTypeManager FileTypeManager { get; set; }

        public ProjectTermsExtractor()
        {
            terms = new List<string>();
            FileTypeManager = DefaultFileTypeManager.CreateInstance(true);
        }

        public void ExtractProjectFileTerms(ProjectFile projectFile, IMultiFileConverter multiFileConverter)
        {
            if (projectFile.Role != FileRole.Translatable) return;

            FileTypeManager.SettingsBundle = Sdl.Core.Settings.SettingsUtil.CreateSettingsBundle(null);

            // disable xliff validation to speed up things
            FileTypeManager.SettingsBundle.GetSettingsGroup("SDL XLIFF 1.0 v 1.0.0.0").GetSetting<bool>("ValidateXliff").Value = false;

            TextExtractionBilingualContentHandler extractor = new TextExtractionBilingualContentHandler();

            multiFileConverter.AddBilingualProcessor(new Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.BilingualContentHandlerAdapter(extractor));
            multiFileConverter.Parse();

            foreach (var text in extractor.Text)
            {
                foreach (var term in GetTerms(text))
                {
                    terms.Add(term);
                }
            }
        }

        protected virtual IEnumerable<string> GetTerms(string text)
        {
            StringBuilder term = new StringBuilder();
            bool containsLetter = false;

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
                    if (term.Length > 1 && containsLetter)
                    {
                        yield return term.ToString();
                    }
                    term.Clear();
                    containsLetter = false;
                }
            }
        }

        public List<string> GetProjectTerms()
        {
            return terms;
        }
    }
}
