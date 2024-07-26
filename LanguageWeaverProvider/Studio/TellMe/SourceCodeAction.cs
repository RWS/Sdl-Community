﻿using System.Drawing;

namespace LanguageWeaverProvider.Studio.TellMe
{
	class SourceCodeAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "source", "code", "github" };
		private static readonly string _actionName = Constants.TellMe_SourceCode_Name;
		private static readonly string _url = Constants.TellMe_SourceCode_Url;
		private static readonly Icon _icon = PluginResources.TellMeSourceCode;
		private static readonly bool _isAvailable = true;

		public SourceCodeAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
	}
}