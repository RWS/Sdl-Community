using System;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.WordCloud.Plugin
{
    [ViewPart(
       Id = "CodingBreeze.WordCloud.ProjectsViewPart",
       Name = "Trados Studio Word Cloud",
       Description = "Show contents of the project in a word cloud.",
       Icon = "wordcloud")]
    [ViewPartLayout(Dock = DockType.Bottom, LocationByType = typeof(ProjectsController))]
    public class WordCloudViewPart : AbstractViewPartController
    {
	    private readonly Lazy<WordCloudViewPartControl> _control = new Lazy<WordCloudViewPartControl>(() => new WordCloudViewPartControl());

		protected override IUIControl GetContentControl()
        {
            return _control.Value;
        }

        protected override void Initialize()
        {
            _control.Value.ViewModel = WordCloudViewModel.Instance;
        }

        internal void GenerateWordCloud()
        {
            _control.Value.GenerateWordCloud();
        }
    }
}