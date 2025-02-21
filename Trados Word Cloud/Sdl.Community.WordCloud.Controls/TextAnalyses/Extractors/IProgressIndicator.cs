namespace Sdl.Community.WordCloud.Controls.TextAnalyses.Extractors
{
    public interface IProgressIndicator
    {
        int Maximum { get; set; }
        void Increment(int value);
    }
}
