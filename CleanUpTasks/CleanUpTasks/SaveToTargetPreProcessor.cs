using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using SDLCommunityCleanUpTasks.Models;
using SDLCommunityCleanUpTasks.Utilities;

namespace SDLCommunityCleanUpTasks
{
	public class SaveToTargetPreProcessor : AbstractBilingualContentHandler
    {
        private readonly ICleanUpSourceSettings sourceSettings = null;
        private readonly ICleanUpTargetSettings targetSettings = null;
        private readonly IXmlReportGenerator reportGenerator = null;
        private ICleanUpMessageReporter reporter = null;

        public SaveToTargetPreProcessor(ICleanUpSourceSettings sourceSettings, ICleanUpTargetSettings targetSettings, IXmlReportGenerator reportGenerator)
        {
            this.sourceSettings = sourceSettings;
            this.targetSettings = targetSettings;
            this.reportGenerator = reportGenerator;
        }

        public override void Initialize(IDocumentProperties documentInfo)
        {
            reporter = new CleanUpMessageReporter(MessageReporter);
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            if (paragraphUnit.IsStructure) { return; }

            foreach (var segPair in paragraphUnit.SegmentPairs)
            {
                var source = segPair.Source;
                var target = segPair.Target;

                // Convert tags back to text in source
                source.AcceptVisitor(new TargetCleanUpHandler(sourceSettings, ItemFactory, reporter));
                // Convert tags back to text in target
                target.AcceptVisitor(new TargetCleanUpHandler(sourceSettings, ItemFactory, reporter));
                target.AcceptVisitor(new ConversionCleanupHandler(targetSettings, LoadConversionFiles(), ItemFactory, reporter, reportGenerator, BatchTaskMode.Target));
            }

            if (targetSettings.ApplyToNonTranslatables)
            {
                var target = paragraphUnit.Target;

                if (target != null)
                {
                    var nonTranslatableHandler = new NonTranslatableHandler(targetSettings, LoadConversionFiles(), reportGenerator);

                    foreach (var item in target)
                    {
                        if (!(item is ISegment))
                        {
                            item.AcceptVisitor(nonTranslatableHandler);
                        }
                    }

                    nonTranslatableHandler.ProcessText();
                }
            }
        }

        public override void SetFileProperties(IFileProperties fileInfo)
        {

            CultureInfo cultureInfo = null;

            try
            {
                var sniffInfo = fileInfo.FileConversionProperties?.FileSnifferInfo;
                cultureInfo = sniffInfo?.DetectedSourceLanguage?.First?.CultureInfo;
            }
            catch (UnsupportedLanguageException)
            {
                // We just ignore these and fall back on oridinal comparison
            }
            finally
            {
                targetSettings.SourceCulture = cultureInfo;
            }
        }

        /// <summary>
        /// Deserializes each conversion file for use in <see cref="ConversionCleanupHandler"/>
        /// </summary>
        /// <returns>A list of <see cref="ConversionItemList"/></returns>
        private List<ConversionItemList> LoadConversionFiles()
        {
            var items = new List<ConversionItemList>(targetSettings.ConversionFiles.Count);

            try
            {
                foreach (var pair in targetSettings.ConversionFiles)
                {
                    // Only load conversion files that exist and are checked
                    if (File.Exists(pair.Key) && pair.Value)
                    {
                        var conversionItemList = XmlUtilities.Deserialize(pair.Key);
                        items.Add(conversionItemList);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                // TODO: Log
            }

            return items;
        }
    }
}