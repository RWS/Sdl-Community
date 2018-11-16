using System.ComponentModel;
using System.Windows.Forms;
using IATETerminologyProvider.Model;

namespace IATETerminologyProvider.Ui
{
	public partial class Settings : Form
	{
		#region Private Fields
		private ProviderSettings _providerSettings;
		#endregion

		#region Constructors
		public Settings()
		{
			InitializeComponent();
		}
		#endregion

		#region Public Methods
		public ProviderSettings GetSettings()
		{
			return _providerSettings;
		}
		#endregion

		#region Private Methods
		protected override void OnClosing(CancelEventArgs e)
		{
			int result;
			var providerSettings = new ProviderSettings
			{
				Limit = int.TryParse(txtBox_Limit.Text, out result) ? int.Parse(txtBox_Limit.Text) : 0,
				Offset = int.TryParse(txtBox_Offset.Text, out result) ? int.Parse(txtBox_Offset.Text) : 0
			};
			_providerSettings = providerSettings;
		}
		#endregion
	}
}