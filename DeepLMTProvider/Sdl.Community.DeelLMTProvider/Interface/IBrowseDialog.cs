namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IBrowseDialog
    {
        string[] FileNames { get; set; }

        bool ShowDialog();
        string ShowSaveDialog();
    }
}