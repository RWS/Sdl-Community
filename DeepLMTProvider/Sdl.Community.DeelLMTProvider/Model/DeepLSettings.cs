namespace Sdl.Community.DeepLMTProvider.Model
{
    public class DeepLSettings(Formality formality, string glossaryId, TagFormat tagHandling, bool preserveFormatting)
    {
        public Formality Formality { get; set; } = formality;
        public string GlossaryId { get; } = glossaryId;
        public bool PreserveFormatting { get; }
        public TagFormat TagHandling { get; } = tagHandling;
    }
}