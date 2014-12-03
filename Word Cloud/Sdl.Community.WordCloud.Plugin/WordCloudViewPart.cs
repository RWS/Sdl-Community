using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.WordCloud.Plugin
{
    [ViewPart(
       Id = "CodingBreeze.WordCloud.ProjectsViewPart",
       Name = "Word Cloud",
       Description = "Show contents of the project in a word cloud.",
       Icon = "wordcloud")]
    [ViewPartLayout(Dock = DockType.Bottom, LocationByType = typeof(ProjectsController))]
    public class WordCloudViewPart : AbstractViewPartController
    {
        protected override System.Windows.Forms.Control GetContentControl()
        {
            return _control.Value;
        }

        protected override void Initialize()
        {
            _control.Value.ViewModel = WordCloudViewModel.Instance;
        }

        private readonly Lazy<WordCloudViewPartControl> _control = new Lazy<WordCloudViewPartControl>(() => new WordCloudViewPartControl());


        internal void GenerateWordCloud()
        {
            _control.Value.GenerateWordCloud();
        }
    }
}
