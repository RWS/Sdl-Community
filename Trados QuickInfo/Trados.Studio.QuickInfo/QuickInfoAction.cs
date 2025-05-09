using Sdl.TellMe.ProviderApi;
using System.Drawing;
using System.Windows.Forms;

namespace TradosStudioQuickInfo
{
    public class QuickInfoAction : ITellMeAction
    {
        private readonly object resultNode;

        public QuickInfoAction()
        {
        }

        public QuickInfoAction(string processorName, string text)
        {
            this.Description = $"Results from {processorName}";
            this.Name = text;
           
        }

        public string Name { get; set; } = "QuickInfo link";

        public string Category => "QuickInfo results";

        public string Description { get; set; } = "Opens a link returned in a quick info search";

        public Icon Icon { get; set; }

        public bool IsAvailable => true;

        public void Execute()
        {
            if(!string.IsNullOrEmpty(Name))
                Clipboard.SetText(Name);
        }
    }
}
