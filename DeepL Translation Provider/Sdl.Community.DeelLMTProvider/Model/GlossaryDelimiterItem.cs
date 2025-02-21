using System.IO;

namespace Sdl.Community.DeepLMTProvider.Model
{
    public class GlossaryDelimiterItem
    {
        public GlossaryDelimiterItem(string filepath, string delimiter)
        {
            Filepath = filepath;
            Delimiter = delimiter;
            Name = Path.GetFileName(filepath);
        }
        public string Delimiter { get; set; }
        public string Name { get; set; }
        public string Filepath { get; set; }
    }
}