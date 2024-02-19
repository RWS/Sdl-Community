﻿using System.Drawing;

namespace LanguageWeaverProvider.Studio.TellMe
{
	class DocumentationAction : TellMeAction
	{
		private static readonly string[] _helpKeywords = { "help", "guide" };
		private static readonly string _actionName = Constants.TellMe_Documentation_Name;
		private static readonly string _url = Constants.TellMe_Documentation_Url;
		private static readonly Icon _icon = PluginResources.TellMeDoc;
		private static readonly bool _isAvailable = true;

		public DocumentationAction() : base(_actionName, _icon, _helpKeywords, _isAvailable, url: _url) { }
	}
}