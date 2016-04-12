using System.Windows.Forms;
using Sdl.Community.NumberVerifier.Properties;

namespace Sdl.Community.NumberVerifier
{
    public partial class NumberVerifierHelp : UserControl
    {


        public NumberVerifierHelp()
        {
            InitializeComponent();
            this.VScroll = true;
        }

        private void NumberVerifierHelp_Load(object sender, System.EventArgs e)
        {
            var assembly = typeof (NumberVerifierHelp).Assembly;
            var resourceName = "Sdl.Community.NumberVerifier.Resources.HelpText.rtf";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                richTextBox1.LoadFile(stream, RichTextBoxStreamType.RichText);
            }
        }
    }
}
