using System.Linq;
using LanguageWeaverProvider.Studio.BatchTask.Send_Feedback;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Bilingual;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    /// <summary>
    /// CRQ-46913: segments whose origin says Language Weaver but that carry no Language Weaver translation
    /// metadata (nmt_model / nmt_translation) made "Send Feedback to Language Weaver" report
    /// "Object reference not set to an instance of an object" per segment. They must be skipped and listed
    /// with an explanation instead.
    /// </summary>
    public class SendFeedbackProcessorTests
    {
        private const string SourceText = "When standard MT fails, NIMBUS delivers!";

        [Theory]
        [InlineData("Language Weaver Cloud", "Cloud")]
        [InlineData("Language Weaver Edge", "Edge")]
        public void SegmentWithoutLanguageWeaverMetadata_IsListedWithExplanation(string originSystem, string provider)
        {
            var processor = new SendFeedbackProcessor();

            processor.ProcessParagraphUnit(CreateParagraphUnit(originSystem, SourceText));

            var error = Assert.Single(processor.Errors);
            Assert.Equal(provider, error.Provider);
            Assert.Equal(SourceText, error.SourceSegment);
            Assert.Equal(
                "Feedback was not sent: no Language Weaver translation data (model and original machine translation) is stored in this segment. Translate the segment again with Language Weaver, then send feedback.",
                error.Error);
        }

        private static IParagraphUnit CreateParagraphUnit(string originSystem, string sourceText)
        {
            var factory = new DocumentItemFactory();
            var paragraphUnit = factory.CreateParagraphUnit(LockTypeFlags.Unlocked);

            var origin = factory.CreateTranslationOrigin();
            origin.OriginType = "nmt";
            origin.OriginSystem = originSystem;

            var properties = factory.CreateSegmentPairProperties();
            properties.Id = new SegmentId("21");
            properties.ConfirmationLevel = ConfirmationLevel.Translated;
            properties.TranslationOrigin = origin;

            var source = factory.CreateSegment(properties);
            source.Add(factory.CreateText(factory.PropertiesFactory.CreateTextProperties(sourceText)));
            var target = factory.CreateSegment(properties);
            target.Add(factory.CreateText(factory.PropertiesFactory.CreateTextProperties("Wenn Standard-MT versagt, liefert NIMBUS!")));

            paragraphUnit.Source.Add(source);
            paragraphUnit.Target.Add(target);
            return paragraphUnit;
        }
    }
}
