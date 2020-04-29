using System;
using System.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace UpdateServerTm
{
	[ApplicationInitializer]
	public class UpdateTm : IApplicationInitializer
	{
		public void Execute()
		{
			var uri = new Uri(@"Add your url");
			var translationProviderServer = new TranslationProviderServer(uri, false, "user name", "password");

			//Add Translation memory id
			var guid = new Guid("");
			var tm = translationProviderServer.GetTranslationMemory(guid, TranslationMemoryProperties.None);

			var langDirection = tm.GetLanguageDirection(new LanguagePair(new CultureInfo("en-us"), new CultureInfo("ro-ro")));
			var iterator = new RegularIterator(10);
			var translationUnits = langDirection.GetTranslationUnits(ref iterator);
			//Add the id of the translation unit you want to update
			var tu = langDirection.GetTranslationUnit(
				new PersistentObjectToken(1, new Guid("")));

			//segment visitor
			var textVisitor = new TextVisitor();
			var segmentElement = tu.TargetSegment.Elements[0];
			segmentElement.AcceptSegmentElementVisitor(textVisitor);	

			langDirection.UpdateTranslationUnit(tu);
			tm.Save();	   	
		}
	}
}
