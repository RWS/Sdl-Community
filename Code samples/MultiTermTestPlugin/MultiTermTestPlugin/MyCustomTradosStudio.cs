using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using MultiTermIX;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace MultiTermTestPlugin
{
	[Action("MultiTermTest")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			Application oMt = new ApplicationClass();
			var localRep = oMt.LocalRepository;
			localRep.Connect("", "");
			var termbases = localRep.Termbases;
			var path = "";// termbase local path
			termbases.Add(path,"","");
			var termbase = termbases[path];
			var entries = termbase.Entries;
			//Concept with terms for English and German
			var entryText =
				"<conceptGrp><languageGrp><language type=\"English\" lang=\"en-US\"></language><termGrp><term>for</term></termGrp></languageGrp><languageGrp><language type=\"German\" lang=\"de-DE\"></language><termGrp><term>für</term></termGrp></languageGrp></conceptGrp>";
			entries.New(entryText, true);

			//var oServerRep = oMt.ServerRepository;
			//oServerRep.Location = "";
			//oServerRep.Connect("", "");
			//Console.WriteLine("Connection successful: " + oServerRep.IsConnected);

			//var oTbs = oServerRep.Termbases[0];
			//oTbs.Entries.New()
		}
	}
}
