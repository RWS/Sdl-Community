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
        ModelType modelType)
    {
        public Formality Formality { get; set; } = formality;
        public string GlossaryId { get; } = glossaryId;
        public List<string> IgnoreTags { get; set; } = ignoreTags;
        public ModelType ModelType { get; set; } = modelType;
        public bool PreserveFormatting { get; } = preserveFormatting;
        public SplitSentences SplitSentencesHandling { get; } = splitSentencesHandling;
        public TagFormat TagHandling { get; } = tagHandling;
    }
}