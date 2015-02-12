using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.FileTypeSupport.Framework.Native;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.EmbeddedContentProcessor.Processor
{
    public class RegexEmbeddedNativeGenerationProcessor : AbstractNativeGenerationContentProcessor
    {
        private const string EmbeddedContentMetaKey = "OriginalEmbeddedContent";

        public override void InlineStartTag(IStartTagProperties tagInfo)
        {
            string originalText = tagInfo.GetMetaData(EmbeddedContentMetaKey);

            if (originalText != null)
            {
                ITextProperties textInfo = PropertiesFactory.CreateTextProperties(originalText);
                base.Text(textInfo);
            }
            else
            {
                base.InlineStartTag(tagInfo);
            }
        }

        public override void InlineEndTag(IEndTagProperties tagInfo)
        {
            string originalText = tagInfo.GetMetaData(EmbeddedContentMetaKey);

            if (originalText != null)
            {
                ITextProperties textInfo = PropertiesFactory.CreateTextProperties(originalText);
                base.Text(textInfo);
            }
            else
            {
                base.InlineEndTag(tagInfo);
            }
        }

        public override void InlinePlaceholderTag(IPlaceholderTagProperties tagInfo)
        {
            string originalText = tagInfo.GetMetaData(EmbeddedContentMetaKey);

            if (originalText != null)
            {
                ITextProperties textInfo = PropertiesFactory.CreateTextProperties(originalText);
                base.Text(textInfo);
            }
            else
            {
                base.InlinePlaceholderTag(tagInfo);
            }
        }
    }
}
