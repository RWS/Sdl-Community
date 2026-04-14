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
        string translationMemoryId = null)
    {
        public Formality Formality { get; set; } = formality;
        public string GlossaryId { get; } = glossaryId;
        public List<string> IgnoreTags { get; } = ignoreTags;
        public ModelType ModelType { get; } = modelType;
        public bool PreserveFormatting { get; } = preserveFormatting;
        public SplitSentences SplitSentencesHandling { get; } = splitSentencesHandling;
        public string StyleId { get; } = styleId;
        public TagFormat TagHandling { get; } = tagHandling;
        public string TranslationMemoryId { get; } = translationMemoryId;
    }
}