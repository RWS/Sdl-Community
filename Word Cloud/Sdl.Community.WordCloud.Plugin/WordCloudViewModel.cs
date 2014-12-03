using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Blacklist;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Extractors;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Processing;
using Sdl.Community.WordCloud.Controls.TextAnalyses.Stemmers;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.WordCloud.Plugin
{
    class WordCloudViewModel
    {
        public static WordCloudViewModel Instance = new WordCloudViewModel();

        public WordCloudViewModel()
        {
            ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
            ProjectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
            OnSelectedProjectsChanged();
        }

        public event EventHandler SelectedProjectChanged;

        private ProjectsController ProjectsController
        {
            get;
            set;
        }

        public FileBasedProject Project
        {
            get;
            set;
        }

        public List<IWord> Words
        {
            get;
            set;
        }

        void ProjectsController_SelectedProjectsChanged(object sender, EventArgs e)
        {
            OnSelectedProjectsChanged();
        }

        private void OnSelectedProjectsChanged()
        {
            List<FileBasedProject> projects = ProjectsController.SelectedProjects.ToList();

            Project = projects.Count == 1 ? projects[0] : null;

            // load cached cloud
            if (Project != null)
            {
                WordCloudCache cache = new WordCloudCache();
                List<IWord>  words;
                if (cache.TryLoad("sourcecloud", Project, out words))
                {
                    Words = words;
                }
                else
                {
                    Words = null;
                }
            }
            else
            {
                Words = null;
            }

            if (SelectedProjectChanged != null)
            {
                SelectedProjectChanged(this, EventArgs.Empty);
            }
        }


        public void GenerateWordCloudAsync(Action<WordCloudResult> resultCallback, Action<int> progressCallback)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;

            FileBasedProject currentProject = Project;
            worker.DoWork += (sender, e) =>
                {
                    WordCloudCache cache = new WordCloudCache();

                    IBlacklist blacklist = DefaultBlacklists.GetDefaultBlacklist(currentProject.GetProjectInfo().SourceLanguage.CultureInfo.TwoLetterISOLanguageName); //new NullBlacklist(); //ComponentFactory.CreateBlacklist(checkBoxExcludeEnglishCommonWords.Checked);
                    IBlacklist customBlacklist = new NullBlacklist(); // CommonBlacklist.CreateFromTextFile(s_BlacklistTxtFileName);

                    IProgressIndicator progress = new ProgressIndicator(); //ComponentFactory.CreateProgressBar(inputType, progressBar);
                    ProjectTextExtractor extractor = new ProjectTextExtractor(currentProject);
                    extractor.Progress += (s, p) => { worker.ReportProgress(p.PercentComplete); };
                    IEnumerable<string> terms = extractor.ExtractWords();  //ComponentFactory.CreateExtractor(inputType, textBox.Text, progress);
                    IWordStemmer stemmer = new NullStemmer(); //ComponentFactory.CreateWordStemmer(checkBoxGroupSameStemWords.Checked);

                    IEnumerable<IWord> rawWords = terms
                        .Filter(blacklist)
                        .Filter(customBlacklist)
                        .CountOccurences();
                    List<IWord> words = rawWords
                            .GroupByStem(stemmer)
                            .SortByOccurences()
                            .Cast<IWord>().ToList();

                    cache.Save("sourcecloud", currentProject, words);
                    e.Result = words;

                    if (Project != null && currentProject.GetProjectInfo().Id == Project.GetProjectInfo().Id)
                    {
                        Words = words;
                    }
                };
            worker.RunWorkerCompleted += (sender, e) =>
                {
                    if (Project != null && currentProject.GetProjectInfo().Id == Project.GetProjectInfo().Id)
                    {
                        WordCloudResult result = new WordCloudResult();
                        if (e.Error != null)
                        {
                            result.Exception = e.Error;
                        }
                        else
                        {
                            result.WeightedWords = (IEnumerable<IWord>)e.Result;
                        }

                        resultCallback(result);
                    }
                };
            worker.ProgressChanged += (sender, e) => {  progressCallback(e.ProgressPercentage); };
            worker.RunWorkerAsync();
        }        
    }
}
