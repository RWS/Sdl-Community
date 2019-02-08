using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.CleanUpTasks.Models;
using Sdl.Community.CleanUpTasks.Utilities;
using Sdl.Core.Globalization;
using Sdl.Core.Settings;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.CleanUpTasks
{
	public class SegmentContentHandler : AbstractBilingualContentHandler
    {
        private readonly IProject project = null;
        private readonly IXmlReportGenerator reportGenerator = null;
        private readonly ICleanUpSourceSettings settings = null;
        private IList<ISegmentHandler> handlers = null;

        public SegmentContentHandler(ICleanUpSourceSettings settings, IProject project, IXmlReportGenerator reportGenerator)
        {
            this.settings = settings;
            this.project = project;
            this.reportGenerator = reportGenerator;
        }

        public override void Complete()
        {
            foreach (var handler in handlers)
            {
                if (handler is ConversionCleanupHandler)
                {
                    var placeholderList = ((ConversionCleanupHandler)handler).PlaceholderList;

                    var allPlaceholders = new List<Placeholder>(settings.Placeholders.Count +
                                                                placeholderList.Count);
                    allPlaceholders.AddRange(settings.Placeholders);
                    allPlaceholders.AddRange(placeholderList);

                    settings.Placeholders = allPlaceholders.Distinct().ToList();
                    project.UpdateSettings(((SettingsGroup)settings).SettingsBundle);
                }
            }
        }

        public override void Initialize(IDocumentProperties documentInfo)
        {
            var messageReporter = new CleanUpMessageReporter(MessageReporter);
            handlers = GetHandlers(messageReporter);
        }

        public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
        {
            // During project creation, SegmentPairs is only available
            // after Pre-Translate files is called
            // Before that Segment Pairs is empty and only IParagraph units exist
            // TODO: Consider adding a processor for IParagraph units before segmentation

            if (paragraphUnit.IsStructure) { return; }

            foreach (var segPair in paragraphUnit.SegmentPairs)
            {
                var source = segPair.Source;
                foreach (var handler in handlers)
                {
                    source.AcceptVisitor(handler);
                }
            }

            if (settings.ApplyToNonTranslatables)
            {
                var source = paragraphUnit.Source;

                if (source != null)
                {
                    var nonTranslatableHandler = new NonTranslatableHandler(settings, LoadConversionFiles(), reportGenerator);

                    foreach (var item in source)
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
                settings.SourceCulture = cultureInfo;
            }
        }

        /// <summary>
        /// Gets the segment handlers.
        /// Can add more here when adding additional handlers
        /// </summary>
        /// <returns></returns>
        private IList<ISegmentHandler> GetHandlers(ICleanUpMessageReporter reporter)
        {
            var handlers = new List<ISegmentHandler>();

            if (settings.UseSegmentLocker)
            {
                handlers.Add(new LockHandler(settings, ItemFactory, reporter, reportGenerator));
            }

            if (settings.UseTagCleaner)
            {
                handlers.Add(new TagHandler(settings, new FormattingVisitor(settings), ItemFactory, reporter, reportGenerator));
            }

            if (settings.UseConversionSettings)
            {
                handlers.Add(new ConversionCleanupHandler(settings, LoadConversionFiles(), ItemFactory, reporter, reportGenerator, BatchTaskMode.Source));
            }

            return handlers;
        }

        /// <summary>
        /// Deserializes each conversion file for use in <see cref="ConversionCleanupHandler"/>
        /// </summary>
        /// <returns>A list of <see cref="ConversionItemList"/></returns>
        private List<ConversionItemList> LoadConversionFiles()
        {
            var items = new List<ConversionItemList>(settings.ConversionFiles.Count);

            try
            {
                foreach (var pair in settings.ConversionFiles)
                {
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