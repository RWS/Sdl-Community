using System;
using System.Collections.Generic;
using System.Text;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.WordCloud.Plugin
{
    class ProjectTextExtractor
    {
        public ProjectTextExtractor(FileBasedProject project)
        {
            Project = project;
            FileTypeManager = DefaultFileTypeManager.CreateInstance(true);          
        }

        public FileBasedProject Project
        {
            get;
            set;
        }

        private IFileTypeManager FileTypeManager
        {
            get;
            set;
        }

        public event EventHandler<ProgressEventArgs> Progress;

        public IEnumerable<string> ExtractWords()
        {
            ProjectFile[] files = Project.GetSourceLanguageFiles();
            int i = 0;
            foreach (ProjectFile file in files)
            {
                if (file.Role != FileRole.Translatable)
                {
                    continue;
                }
                FileTypeManager.SettingsBundle = Sdl.Core.Settings.SettingsUtil.CreateSettingsBundle(null);
                
                // disable xliff validation to speed up things
                FileTypeManager.SettingsBundle.GetSettingsGroup("SDL XLIFF 1.0 v 1.0.0.0").GetSetting<bool>("ValidateXliff").Value = false;
                
                IMultiFileConverter converter = FileTypeManager.GetConverter(file.LocalFilePath, (sender, e) => { });
                TextExtractionBilingualContentHandler extractor = new TextExtractionBilingualContentHandler();
                converter.AddBilingualProcessor(new Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi.BilingualContentHandlerAdapter(extractor));
                converter.Parse();
                foreach (string text in extractor.Text)
                {
                    foreach (string word in GetWords(text))
                    {
                        yield return word;
                    }
                }

                i++;
                OnProgress((int)(100.0 * i / (double)files.Length));
            }
        }

        private void OnProgress(int percent)
        {
            if (Progress != null)
            {
                Progress(this, new ProgressEventArgs { PercentComplete = percent });
            }
        }

        protected virtual IEnumerable<string> GetWords(string text)
        {
            StringBuilder word = new StringBuilder();
            bool containsLetter = false;
            
            foreach (char ch in text)
            {
                if (char.IsLetter(ch))
                {
                    word.Append(ch);
                    containsLetter = true;
                }
                else if (char.IsDigit(ch))
                {
                    word.Append(ch);
                }
                else
                {
                    if (word.Length > 1 && containsLetter)
                    {
                        yield return word.ToString();
                    }
                    word.Clear();
                    containsLetter = false;
                }
            }
        }

    }
}
