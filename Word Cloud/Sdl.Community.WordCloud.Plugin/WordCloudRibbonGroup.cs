﻿using System;
using System.Linq;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.WordCloud.Plugin
{
	[RibbonGroup("CodingBreeze.WordCloudRibbonGroup", Name = "Trados Studio Word Cloud", ContextByType = typeof(ProjectsController))]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.HomeRibbonTabLocation))]
	class WordCloudRibbonGroup : AbstractRibbonGroup
	{
	}

	[Action("CodingBreeze.WordCloud.GenerateWordCloudAction", Name = "Create Trados Studio Word Cloud...", Icon = "wordcloud", Description = "Generate a word cloud based on this project's content...")]
	[ActionLayout(typeof(WordCloudRibbonGroup), 200, DisplayType.Large)]
	class GenerateWordCloudAction : AbstractAction
	{

		public GenerateWordCloudAction()
		{
		}

		public override void Initialize()
		{
			ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			ProjectsController.SelectedProjectsChanged += ProjectsController_SelectedProjectsChanged;
			OnSelectedProjectsChanged();
		}

		void ProjectsController_SelectedProjectsChanged(object sender, EventArgs e)
		{
			OnSelectedProjectsChanged();
		}

		private void OnSelectedProjectsChanged()
		{
			Enabled = ProjectsController.SelectedProjects.Count() == 1;
		}

		private ProjectsController ProjectsController
		{
			get;
			set;
		}

		private void UpdateEnabled()
		{

		}

		protected override void Execute()
		{
			WordCloudViewPart wc = SdlTradosStudio.Application.GetController<WordCloudViewPart>();
			wc.Activate();
			wc.GenerateWordCloud();
		}
	}

	[Action("CodingBreeze.WordCloud.Help", Name = "Help", Icon = "help_icon", Description = "Help")]
	[ActionLayout(typeof(WordCloudRibbonGroup), 20, DisplayType.Large)]
	class HelpLinkAction : AbstractAction
	{
		protected override void Execute()
		{
			System.Diagnostics.Process.Start("https://community.sdl.com/product-groups/translationproductivity/w/customer-experience/3289/trados-studio-word-cloud");
		}
	}
}