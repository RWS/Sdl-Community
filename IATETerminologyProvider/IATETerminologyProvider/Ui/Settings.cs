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
			//var providerSettings = new ProviderSettings
			//{
			//	AllDomains = ckb_AllDomains.Checked,
			//	NoDomains = ckb_NoDomains.Checked,
			//	NoDuplicates = ckb_NoDuplicates.Checked,
			//};
			//_providerSettings = providerSettings;
		}
		
		private void ckb_NoDomains_CheckedChanged(object sender, System.EventArgs e)
		{
			if (ckb_NoDomains.Checked)
			{
				ckb_AllDomains.Enabled = false;
				ckb_NoDuplicates.Enabled = false;
			}
			else
			{
				ckb_AllDomains.Enabled = true;
				ckb_NoDuplicates.Enabled = true;
			}
		}

		private void ckb_AllDomains_CheckedChanged(object sender, System.EventArgs e)
		{
			if (ckb_AllDomains.Checked)
			{
				ckb_NoDomains.Enabled = false;
				ckb_NoDuplicates.Enabled = false;
			}
			else
			{
				ckb_NoDomains.Enabled = true;
				ckb_NoDuplicates.Enabled = true;
			}
		}

		private void ckb_NoDuplicates_CheckedChanged(object sender, System.EventArgs e)
		{
			if (ckb_NoDuplicates.Checked)
			{
				ckb_NoDomains.Enabled = false;
				ckb_AllDomains.Enabled = false;
			}
			else
			{
				ckb_NoDomains.Enabled = true;
				ckb_AllDomains.Enabled = true;
			}
		}
		#endregion
	}
}