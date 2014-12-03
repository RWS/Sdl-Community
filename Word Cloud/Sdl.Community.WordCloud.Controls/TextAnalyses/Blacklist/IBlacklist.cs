namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Blacklist
{
    public interface IBlacklist
    {
        bool Countains(string word);
        int Count { get; }
    }
}
