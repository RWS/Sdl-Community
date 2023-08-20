namespace Sdl.Community.DeepLMTProvider.Interface
{
    public interface IOpenFileDialog
    {
        string[] FileNames { get; set; }

        bool ShowDialog();
    }
}