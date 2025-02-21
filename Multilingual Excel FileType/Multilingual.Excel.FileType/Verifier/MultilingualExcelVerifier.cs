using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Multilingual.Excel.FileType.Constants;
using Multilingual.Excel.FileType.FileType.Settings;
using Multilingual.Excel.FileType.Models;
using Multilingual.Excel.FileType.Services;
using Multilingual.Excel.FileType.Services.Entities;
using Multilingual.Excel.FileType.Verifier.MessageUI;
using Multilingual.Excel.FileType.Verifier.Settings;
using Sdl.Core.PluginFramework;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Sdl.Verification.Api;

namespace Multilingual.Excel.FileType.Verifier
{
	[GlobalVerifier(SettingsConstants.MultilingualExcelVerifierId, "Verifier_Name", "Verifier_Description")]
	public class MultilingualExcelVerifier : IGlobalVerifier, IBilingualVerifier, ISharedObjectsAware
	{
		private IDocumentItemFactory _itemFactory;
		private IBilingualContentMessageReporter _messageReporter;

		private ISharedObjects _sharedObjects;
		private MultilingualExcelVerificationSettings _verificationSettings;
		private LanguageMappingSettings _languageMappingSettings;
		private TextGenerator _textGeneratorProcessor;
		private IDocumentProperties _documentProperties;
		private IFileProperties _fileInfo;
		private SegmentVisitor _segmentVisitor;
		private EntityService _entityService;
		private IDocumentItemFactory _documentItemFactory;
		private IPropertiesFactory _propertiesFactory;
		private EntityContext _entityContext;
		private SdlFrameworkService _sdlFrameworkService;
		private EntityMarkerConversionService _entityMarkerConversionService;
		private BilingualContentMessageReporterProxy _bilingualContentMessageReporterProxy;

		public IList<string> GetSettingsPageExtensionIds()
		{
			IList<string> list = new List<string>
			{
				SettingsConstants.MultilingualExcelVerificationSettingsId
			};

			return list;
		}

		internal MultilingualExcelVerificationSettings VerificationSettings
		{
			get
			{
				if (_verificationSettings != null || _sharedObjects == null)
				{
					return _verificationSettings;
				}

				var bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
				if (bundle != null)
				{
					_verificationSettings = bundle.GetSettingsGroup<MultilingualExcelVerificationSettings>();
				}

				return _verificationSettings;
			}
		}

		internal LanguageMappingSettings LanguageMappingSettings
		{
			get
			{
				if (_languageMappingSettings != null || _sharedObjects == null)
				{
					return _languageMappingSettings;
				}

				var bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
				if (bundle != null)
				{
					_languageMappingSettings = new LanguageMappingSettings();
					_languageMappingSettings.PopulateFromSettingsBundle(bundle, FiletypeConstants.FileTypeDefinitionId);
				}

				return _languageMappingSettings;
			}
		}

		internal LanguageMapping LanguageMapping { get; set; }

		public TextGenerator TextGeneratorProcessor => _textGeneratorProcessor ?? (_textGeneratorProcessor = new TextGenerator());

		public string Description => PluginResources.Verifier_Description;

		public Icon Icon => PluginResources.MLExcel;

		public string Name => PluginResources.Verifier_Name;

		public string SettingsId => SettingsConstants.MultilingualExcelVerifierSettings;

		public string HelpTopic => string.Empty;

		public bool IsSubtitlingFormat { get; private set; }

		public bool Enabled { get; private set; }

		public static Dictionary<string, bool> GetVerifiers(ISettingsBundle settingsBundle)
		{
			if (settingsBundle == null)
			{
				return null;
			}

			var verifiers = new Dictionary<string, bool>();
			var globalVerifiersExtensionPoint = PluginManager.DefaultPluginRegistry.GetExtensionPoint<GlobalVerifierAttribute>();
			if (globalVerifiersExtensionPoint == null)
			{
				return null;
			}

			foreach (var extension in globalVerifiersExtensionPoint.Extensions)
			{
				var globalVerifier = (IGlobalVerifier)extension.CreateInstance();
				if (globalVerifier.SettingsId == null)
				{
					continue;
				}

				var settingsGroup = settingsBundle.GetSettingsGroup(globalVerifier.SettingsId);
				verifiers.Add(globalVerifier.SettingsId, settingsGroup.GetSetting("Enabled", false).Value);
			}

			return verifiers;
		}

		public IDocumentItemFactory ItemFactory
		{
			get => _itemFactory;
			set => _itemFactory = value;
		}

		public IBilingualContentMessageReporter MessageReporter
		{
			get => _messageReporter;
			set => _messageReporter = value;
		}

		private BilingualContentMessageReporterProxy MessageReporterProxy
		{
			get
			{
				if (_bilingualContentMessageReporterProxy == null)
				{
					_bilingualContentMessageReporterProxy = new BilingualContentMessageReporterProxy(MessageReporter);
				}

				return _bilingualContentMessageReporterProxy;
			}
		}

		public void Initialize(IDocumentProperties documentInfo)
		{
			_documentProperties = documentInfo;
			Enabled = IsEnabled();

			_documentItemFactory = DefaultDocumentItemFactory.CreateInstance();
			_propertiesFactory = DefaultPropertiesFactory.CreateInstance();

			_entityContext = new EntityContext();
			_sdlFrameworkService = new SdlFrameworkService(_documentItemFactory, _propertiesFactory);
			_entityMarkerConversionService = new EntityMarkerConversionService();

			_entityService = new EntityService(_entityContext, _sdlFrameworkService, _entityMarkerConversionService);

			_segmentVisitor = new SegmentVisitor(_entityService, true);

			LanguageMapping = LanguageMappingSettings.LanguageMappingLanguages.FirstOrDefault(a =>
				a.LanguageId == _documentProperties.TargetLanguage.CultureInfo.Name);
		}

		public void Complete()
		{
			//throw new NotImplementedException();
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			_fileInfo = fileInfo;
		}

		public void FileComplete()
		{
			//throw new NotImplementedException();
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (!Enabled)
			{
				return;
			}

			CheckParagraphUnit(paragraphUnit);
		}

		public void SetSharedObjects(ISharedObjects sharedObjects)
		{
			_sharedObjects = sharedObjects;
		}

		public bool IsEnabled()
		{
			var bundle = _sharedObjects.GetSharedObject<ISettingsBundle>("SettingsBundle");
			if (bundle == null)
			{
				return false;
			}

			var verifiers = GetVerifiers(bundle);
			return verifiers.ContainsKey(SettingsId) && verifiers[SettingsId];
		}

		private void CheckParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (LanguageMapping == null)
			{
				return;
			}



			if (paragraphUnit.SegmentPairs != null && paragraphUnit.SegmentPairs.Any())
			{
				if (VerificationSettings.MaxCharacterLengthEnabled
					|| VerificationSettings.MaxPixelLengthEnabled
					|| VerificationSettings.MaxLinesPerParagraphEnabled)
				{
					var multilingualParagraphUnitContext = paragraphUnit.Properties.Contexts?.Contexts.FirstOrDefault(a => a.ContextType == FiletypeConstants.MultilingualParagraphUnit);
					if (multilingualParagraphUnitContext != null)
					{
						var excelCharacterLimitationSource = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelCharacterLimitationSource) ?? "0");
						var excelLineLimitationSource = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelLineLimitationSource) ?? "0");
						var excelPixelLimitationSource = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelLimitationSource) ?? "0");
						var excelPixelFontNameSource = multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontNameSource) ?? string.Empty;
						var excelPixelFontSizeSource = Convert.ToSingle(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontSizeSource) ?? "0");

						var excelCharacterLimitationTarget = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelCharacterLimitationTarget) ?? "0");
						var excelLineLimitationTarget = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelLineLimitationTarget) ?? "0");
						var excelPixelLimitationTarget = Convert.ToInt32(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelLimitationTarget) ?? "0");
						var excelPixelFontNameTarget = multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontNameTarget) ?? string.Empty;
						var excelPixelFontSizeTarget = Convert.ToSingle(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.MultilingualExcelPixelFontSizeTarget) ?? "0");
						var isCDATA = Convert.ToBoolean(multilingualParagraphUnitContext.GetMetaData(FiletypeConstants.IsCDATA));


						if (excelCharacterLimitationTarget == 0)
						{
							excelCharacterLimitationTarget = excelCharacterLimitationSource;
						}
						if (excelLineLimitationTarget == 0)
						{
							excelLineLimitationTarget = excelLineLimitationSource;
						}
						if (excelPixelLimitationTarget == 0)
						{
							excelPixelLimitationTarget = excelPixelLimitationSource;
						}
						if (string.IsNullOrEmpty(excelPixelFontNameTarget))
						{
							excelPixelFontNameTarget = excelPixelFontNameSource;
						}
						if (excelPixelFontSizeTarget == 0)
						{
							excelPixelFontSizeTarget = excelPixelFontSizeSource;
						}

						var sourceContent = string.Empty;
						var targetContent = string.Empty;
						foreach (var segmentPair in paragraphUnit.SegmentPairs)
						{
							_segmentVisitor.IgnoreTags = true;
							_segmentVisitor.VisitSegment(segmentPair.Source);
							sourceContent += _segmentVisitor.Text;

							_segmentVisitor.VisitSegment(segmentPair.Target);
							targetContent += _segmentVisitor.Text;
						}

						if (VerificationSettings.VerifySourceParagraphsEnabled && !string.IsNullOrEmpty(sourceContent))
						{
							if (VerificationSettings.MaxCharacterLengthEnabled && excelCharacterLimitationSource > 0   && sourceContent.Length > excelCharacterLimitationSource)
							{
								foreach (var segmentPair in paragraphUnit.SegmentPairs)
								{
									var segmentPairId = segmentPair?.Properties?.Id.Id;
									if (segmentPairId == null)
									{
										continue;
									}

									var message = string.Format(PluginResources.VerficationMessage_CharacterLengthExceedsMaximum, 
										"Source", sourceContent.Length, excelCharacterLimitationSource);

									var extendedMessageData = MultilingualExcelMessageData(segmentPair, message, "MaxCharacterLength");

									MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
										GetErrorLevel(VerificationSettings.MaxCharacterLengthSeverity), message,
										new TextLocation(new Location(segmentPair.Source, true), 0),
										new TextLocation(new Location(segmentPair.Source, false), (segmentPair.Source.ToString().Length) - 1),
										extendedMessageData);
								}
							}

							if (VerificationSettings.MaxLinesPerParagraphEnabled && excelLineLimitationSource > 0 && sourceContent.Length > excelLineLimitationSource)
							{
								var regexLine = new Regex(@"\r\n|\r|\n", RegexOptions.Singleline);
								var lpp = regexLine.Split(sourceContent).Length;

								if (lpp > excelLineLimitationSource)
								{
									foreach (var segmentPair in paragraphUnit.SegmentPairs)
									{
										var segmentPairId = segmentPair?.Properties?.Id.Id;
										if (segmentPairId == null)
										{
											continue;
										}

										var message = string.Format(PluginResources.VerificationMessage_ParagraphLinesExceedsMaximum,
											"Source",
											lpp,
											excelLineLimitationSource);

										var extendedMessageData = MultilingualExcelMessageData(segmentPair, message, "MaxLinesPerParagraph");

										MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
											GetErrorLevel(VerificationSettings.MaxLinesPerParagraphSeverity), message,
											new TextLocation(new Location(segmentPair.Source, true), 0),
											new TextLocation(new Location(segmentPair.Source, false),
												(segmentPair.Source.ToString().Length) - 1),
											extendedMessageData);
									}
								}
							}

							if (VerificationSettings.MaxPixelLengthEnabled && excelPixelLimitationSource > 0)
							{
								var fontFamily = GetFontFamily(excelPixelFontNameSource, out var useDefaultFont);
								var fontSize = GetFontSize(excelPixelFontSizeSource, out var useDefaultFontSize);

								var pixelLength = Math.Round(GetPixelWidth(sourceContent, fontFamily, fontSize), 2);

								if (pixelLength > excelPixelLimitationSource)
								{
									foreach (var segmentPair in paragraphUnit.SegmentPairs)
									{
										var segmentPairId = segmentPair?.Properties?.Id.Id;
										if (segmentPairId == null)
										{
											continue;
										}

										var message = string.Format(PluginResources.VerficationMessage_PixelLengthExceedsMaximum,
											"Source", pixelLength, excelPixelLimitationSource,
											useDefaultFont
												? PluginResources.VerificationMessage_DefaultInParenthesis + " "
												: string.Empty, fontFamily.Name,
											useDefaultFontSize
												? PluginResources.VerificationMessage_DefaultInParenthesis + " "
												: string.Empty, fontSize);

										var extendedMessageData = MultilingualExcelMessageData(segmentPair, message, "MaxPixelLength");

										MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
											GetErrorLevel(VerificationSettings.MaxPixelLengthSeverity), message,
											new TextLocation(new Location(segmentPair.Source, true), 0),
											new TextLocation(new Location(segmentPair.Source, false), (segmentPair.Source.ToString().Length) - 1),
											extendedMessageData);
									}
								}
							}


						}

						if (VerificationSettings.VerifyTargetParagraphsEnabled && !string.IsNullOrEmpty(targetContent))
						{
							if (VerificationSettings.MaxCharacterLengthEnabled && excelCharacterLimitationTarget > 0  && targetContent.Length > excelCharacterLimitationTarget)
							{
								foreach (var segmentPair in paragraphUnit.SegmentPairs)
								{
									var segmentPairId = segmentPair?.Properties?.Id.Id;
									if (segmentPairId == null)
									{
										continue;
									}

									var message = string.Format(PluginResources.VerficationMessage_CharacterLengthExceedsMaximum, 
										"Target", targetContent.Length, excelCharacterLimitationTarget);

									var extendedMessageData = MultilingualExcelMessageData(segmentPair, message, "MaxCharacterLength");

									MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
										GetErrorLevel(VerificationSettings.MaxCharacterLengthSeverity), message,
										new TextLocation(new Location(segmentPair.Target, true), 0),
										new TextLocation(new Location(segmentPair.Target, false), segmentPair.Target.ToString().Length - 1),
										extendedMessageData);
								}
							}

							if (VerificationSettings.MaxLinesPerParagraphEnabled && excelLineLimitationTarget > 0 && sourceContent.Length > excelLineLimitationTarget)
							{
								var regexLine = new Regex(@"\r\n|\r|\n", RegexOptions.Singleline);
								var lpp = regexLine.Split(targetContent).Length;

								if (lpp > excelLineLimitationTarget)
								{
									foreach (var segmentPair in paragraphUnit.SegmentPairs)
									{
										var segmentPairId = segmentPair?.Properties?.Id.Id;
										if (segmentPairId == null)
										{
											continue;
										}

										var message = string.Format(PluginResources.VerificationMessage_ParagraphLinesExceedsMaximum,
											"Target",
											lpp,
											excelLineLimitationTarget);

										var extendedMessageData = MultilingualExcelMessageData(segmentPair, message, "MaxLinesPerParagraph");

										MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
											GetErrorLevel(VerificationSettings.MaxLinesPerParagraphSeverity), message,
											new TextLocation(new Location(segmentPair.Target, true), 0),
											new TextLocation(new Location(segmentPair.Target, false),
												(segmentPair.Target.ToString().Length) - 1),
											extendedMessageData);
									}
								}
							}

							if (VerificationSettings.MaxPixelLengthEnabled && excelPixelLimitationTarget > 0)
							{

								var fontFamily = GetFontFamily(excelPixelFontNameTarget, out var useDefaultFont);
								var fontSize = GetFontSize(excelPixelFontSizeTarget, out var useDefaultFontSize);

								var pixelLength = Math.Round(GetPixelWidth(targetContent, fontFamily, fontSize), 2);

								if (pixelLength > excelPixelLimitationTarget)
								{
									foreach (var segmentPair in paragraphUnit.SegmentPairs)
									{
										var segmentPairId = segmentPair?.Properties?.Id.Id;
										if (segmentPairId == null)
										{
											continue;
										}

										var message = string.Format(PluginResources.VerficationMessage_PixelLengthExceedsMaximum, 
											"Target", pixelLength, excelPixelLimitationTarget,
											useDefaultFont
												? PluginResources.VerificationMessage_DefaultInParenthesis + " "
												: string.Empty, fontFamily.Name,
											useDefaultFontSize
												? PluginResources.VerificationMessage_DefaultInParenthesis + " "
												: string.Empty, fontSize);

										var extendedMessageData = MultilingualExcelMessageData(segmentPair, message, "MaxPixelLength");

										MessageReporterProxy.ReportMessage(this, PluginResources.Plugin_Name,
											GetErrorLevel(VerificationSettings.MaxPixelLengthSeverity), message,
											new TextLocation(new Location(segmentPair.Target, true), 0),
											new TextLocation(new Location(segmentPair.Target, false), segmentPair.Target.ToString().Length - 1),
											extendedMessageData);
									}
								}
							}
						}
					}
				}
			}
		}

		private MultilingualExcelMessageData MultilingualExcelMessageData(ISegmentPair segmentPair, string message, string messageType)
		{
			var sourceSegmentPlainText = GetSegmentContent(segmentPair.Source, false);
			var targetSegmentPlainText = GetSegmentContent(segmentPair.Target, false);

			var extendedMessageData = new MultilingualExcelMessageData(message);
			extendedMessageData.SourceSegmentPlainText = sourceSegmentPlainText;
			extendedMessageData.TargetSegmentPlainText = targetSegmentPlainText;
			extendedMessageData.MessageType = "MultilingualExcel.MessageUI, "+ messageType;

			return extendedMessageData;
		}

		private float GetFontSize(float excelPixelFontSize, out bool useDefault)
		{
			useDefault = excelPixelFontSize <= 0;
			var fontSize = useDefault ? Convert.ToSingle(LanguageMapping.PixelFontSize) : excelPixelFontSize;

			return fontSize;
		}

		private FontFamily GetFontFamily(string excelPixelFontName, out bool useDefault)
		{
			useDefault = true;
			if (!string.IsNullOrEmpty(excelPixelFontName))
			{
				try
				{
					var fontFamily = new FontFamily(excelPixelFontName);
					useDefault = false;
					return fontFamily;
				}
				catch
				{
					// ignore; catch all
				}
			}

			return new FontFamily(LanguageMapping.PixelFontFamilyName);
		}

		private string GetSegmentContent(ISegment segment, bool ignoreTags)
		{
			if (segment != null)
			{
				_segmentVisitor.IgnoreTags = ignoreTags;
				_segmentVisitor.VisitSegment(segment);
				var sourceContent = _segmentVisitor.Text;
				return sourceContent;
			}

			return null;
		}

		private static string ExecutingStudioLocation()
		{
			var entryAssembly = Assembly.GetEntryAssembly()?.Location;
			var location = entryAssembly?.Substring(0, entryAssembly.LastIndexOf(@"\", StringComparison.Ordinal));

			return location;
		}

		private float GetPixelWidth(string text, FontFamily fontFamily, float fontSize)
		{
			using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
			{
				var size = graphics.MeasureString(text, new Font(fontFamily, fontSize, FontStyle.Regular, GraphicsUnit.Pixel));
				return size.Width;
			}
		}

		private static ErrorLevel GetErrorLevel(int index)
		{
			switch (index)
			{
				case 0: return ErrorLevel.Error;
				case 1: return ErrorLevel.Warning;
				case 2: return ErrorLevel.Note;
				default:
					return ErrorLevel.Warning;
			}
		}
	}


}
