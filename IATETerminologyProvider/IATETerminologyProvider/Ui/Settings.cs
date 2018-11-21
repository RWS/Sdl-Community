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
			var providerSettings = new ProviderSettings
			{
				AllDomains = ckb_AllDomains.Checked,
				NoDomains = ckb_NoDomains.Checked,
				NoDuplicates = ckb_NoDuplicates.Checked,
			};
			_providerSettings = providerSettings;
		}
		#endregion
	}
}