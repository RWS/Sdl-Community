using System.Windows.Forms;

namespace GlobalVerifierSample
{
	public partial class IdenticalVerifierUI : UserControl
	{
		// Data binding for the text field control
		public string ContextToCheck
		{
			get
			{
				return _txtContext.Text;
			}
			set
			{
				_txtContext.Text = value;
			}
		}

		public IdenticalVerifierUI()
		{
			InitializeComponent();
		}
	}
}