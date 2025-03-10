using System.Collections.Generic;
using System.Windows.Documents;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeepLSettings(
        Formality formality, 
        string glossaryId, 
        TagFormat tagHandling, 
        SplitSentences splitSentencesHandling,
        bool preserveFormatting, 
        List<string> ignoreTags)
    {
        public Formality Formality { get; set; } = formality;
        public string GlossaryId { get; } = glossaryId;
        public bool PreserveFormatting { get; } = preserveFormatting;
        public TagFormat TagHandling { get; } = tagHandling;
        public SplitSentences SplitSentencesHandling { get; } = splitSentencesHandling;
        public List<string> IgnoreTags { get; set; } = ignoreTags;
    }
}