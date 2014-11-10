using System.Windows.Forms;

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
            richTextBox1.Rtf = PluginResources.Help_Text;
        }
    }
}
