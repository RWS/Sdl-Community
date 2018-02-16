using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;
using Sdl.Utilities.SplitSDLXLIFF;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.SDLXLIFFSplitMerge
{
	[RibbonGroup("SDLXLIFFSplitMerge", Name = "SDLXLIFF Split & Merge")]
	[RibbonGroupLayout(LocationByType = typeof(TranslationStudioDefaultRibbonTabs.AddinsRibbonTabLocation))]
	public class SDLXLIFFSplitMergeRibbon : AbstractRibbonGroup
	{
		[Action("Sdl.Community.SDLXLIFFSplitMerge", Name = "SDLXLIFF Split & Merge", Icon = "SplitMerge_Icon", Description = "SDLXLIFF Split & Merge")]
		[ActionLayout(typeof(SDLXLIFFSplitMergeRibbon), 20, DisplayType.Large)]
		[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation), 10, DisplayType.Large)]
		public class SDLXLIFFSplitMergeAction : AbstractAction
		{
			protected override void Execute()
			{
				WizardPage wizardPage = new WizardPage();
				wizardPage.ShowDialog();

				//CultureInfo _Culture = new CultureInfo("EN-US");
				//Segment _LinguaSegment = new Segment(_Culture);
				//SegmentElement _textSDL = new Text("test");
				////FileTypeSupport.Framework.BilingualApi.IText _iTxt = (FileTypeSupport.Framework.BilingualApi.IText) _textSDL;

				////get assembly
				//var translationMemoryToolsAssembly =
				//	Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.LanguagePlatform.TranslationMemoryTools.dll"));


				////get object type 
				//var linguaSegmentBuilderType =
				//	translationMemoryToolsAssembly.GetType("Sdl.LanguagePlatform.TranslationMemoryTools.LinguaSegmentBuilder");

				////create constructor type
				//Type[] constructorArgumentTypes = {typeof(Segment), typeof(bool), typeof(bool)};

				////get constructor
				//ConstructorInfo linguaSegmentConstrutor = linguaSegmentBuilderType.GetConstructor(constructorArgumentTypes);

				//// invoke constructor with its arguments
				//dynamic builder = linguaSegmentConstrutor.Invoke(new object[] {_LinguaSegment, false, false});

				////set values to builder
				////builder.VisitText(_iTxt);
				//builder.Result.Elements.Add(_textSDL);


				//var languageProcessingAssembly = Assembly.LoadFrom(Path.Combine(ExecutingStudioLocation(), "Sdl.Core.LanguageProcessing.dll"));
				////get object type
				//var tokenizationFactoryType = languageProcessingAssembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.TokenizerSetupFactory");
				//dynamic tokenizerFactory = tokenizationFactoryType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public |BindingFlags.Static);

				//var createMethod = tokenizerFactory[0];
				//if (createMethod != null)
				//{
				//	dynamic setup = createMethod.Invoke(null, new object[]{_Culture});

				//	setup.CreateWhitespaceTokens = true;
				//	setup.BuiltinRecognizers =
				//		Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeNumbers |
				//		Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeDates |
				//		Sdl.LanguagePlatform.Core.Tokenization.BuiltinRecognizers.RecognizeTimes;



				//	var tokenizerType = languageProcessingAssembly.GetType("Sdl.Core.LanguageProcessing.Tokenization.Tokenizer");
				//	Type[] constructorTokenizerArgumentTypes = {setup.GetType()};
				//	ConstructorInfo tokenizerConstructor = tokenizerType.GetConstructor(constructorTokenizerArgumentTypes);
				//	dynamic tokenizer = tokenizerConstructor.Invoke(new object[] {setup});

				//	IList<Sdl.LanguagePlatform.Core.Tokenization.Token> _tokens = tokenizer.Tokenize(_LinguaSegment);
				//	int wordsNum = 0;
				//	foreach (LanguagePlatform.Core.Tokenization.Token _token in _tokens)
				//	{
				//		if (_token.IsWord)
				//		{
				//			wordsNum++;
				//		}
				//	}
				//}
			}

			/// <summary>
			/// Get Studio location
			/// </summary>
			/// <returns></returns>
			private static string ExecutingStudioLocation()
			{
				var entryAssembly = Assembly.GetEntryAssembly().Location;
				var location = entryAssembly.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

				return location;
			}
		}
	}
}