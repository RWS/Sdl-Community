namespace Sdl.Community.GSVersionFetch.Events
{
    public class ProgressEvent
    {

        public ProgressEvent(string message, bool showRing)
        {
            Message = message;
            ShowRing = showRing;
        }

        public string Message { get; set; }
        public bool ShowRing { get; set; }
    }
}