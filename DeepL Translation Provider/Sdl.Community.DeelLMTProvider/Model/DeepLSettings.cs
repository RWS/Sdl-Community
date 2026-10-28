using System.Collections.Generic;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeepLSettings(
        Formality formality,
        string glossaryId,
        TagFormat tagHandling,
        SplitSentences splitSentencesHandling,
        bool preserveFormatting,
        List<string> ignoreTags,
        ModelType modelType,
        string styleId,
        string translationMemoryId = null,
        TagHandlingVersion tagHandlingVersion = TagHandlingVersion.V2)
    {
        public Formality Formality { get; set; } = formality;
        public string GlossaryId { get; } = glossaryId;
        public List<string> IgnoreTags { get; } = ignoreTags;
        public ModelType ModelType { get; } = modelType;
        public bool PreserveFormatting { get; } = preserveFormatting;
        public SplitSentences SplitSentencesHandling { get; } = splitSentencesHandling;
        public string StyleId { get; } = styleId;
        public TagFormat TagHandling { get; } = tagHandling;
        public TagHandlingVersion TagHandlingVersion { get; } = tagHandlingVersion;
        public string TranslationMemoryId { get; } = translationMemoryId;
    }
}