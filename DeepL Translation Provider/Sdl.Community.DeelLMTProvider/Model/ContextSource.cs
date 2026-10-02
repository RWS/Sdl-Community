namespace Sdl.Community.DeepLMTProvider.Model
{
    // What is sent to DeepL as `context`: nothing, the surrounding source segments,
    // or a text the user writes (custom context) — never both.
    public enum ContextSource
    {
        None,
        SurroundingSegments,
        CustomContext
    }
}
