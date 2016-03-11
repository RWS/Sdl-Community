namespace Sdl.Community.ContentConnector
{
    public class ProjectRequest
    {
        public string Name
        {
            get;
            set;
        }

        public string[] Files
        {
            get; set;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
