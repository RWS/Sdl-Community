using Sdl.Community.Utilities.TMTool.Task;
using System;
using System.Windows.Forms;

namespace Sdl.Community.Utilities.TMTool.Tasks.RemapTMX
{
	public partial class RemapTMXControl : UserControl, IControl
	{
		#region Fields

		/// <summary>
		/// Settings for RemapTMX task.
		/// </summary>
		private RemapTMXSettings remapSettings;

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the RemapTMXControl class.
		/// </summary>
		public RemapTMXControl()
		{
			this.InitializeComponent();

			this.remapSettings = new RemapTMXSettings();

			this.UpdateUI(this.remapSettings);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a reference to self.
		/// </summary>
		public UserControl UControl
		{
			get
			{
				return this;
			}
		}

		/// <summary>
		/// Gets RemapTMX task options.
		/// </summary>
		public ISettings Options
		{
			get
			{
				this.GetUIData();
				return this.remapSettings;
			}

			private set
			{
				if (value is RemapTMXSettings)
				{
					this.remapSettings = (RemapTMXSettings)value;
					this.UpdateUI(this.remapSettings);
				}
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Resets UI to its default values.
		/// </summary>
		public void ResetUI()
		{
			this.remapSettings.ResetToDefaults();
			this.UpdateUI(this.remapSettings);
		}

		/// <summary>
		/// Updates UI with current settings values.
		/// </summary>
		/// <param name="settings">Settings to be set into UI.</param>
		public void UpdateUI(ISettings settings)
		{
			this.outputFolderBox.Text = this.remapSettings.TargetFolder;
			this.saveTargetFolderCheckBox.Checked = this.remapSettings.SaveIntoTargetFolder;
		}

		#endregion

		#region Privates

		/// <summary>
		/// Gets UI data.
		/// </summary>
		private void GetUIData()
		{
			this.remapSettings.TargetFolder = this.outputFolderBox.Text;
			this.remapSettings.SaveIntoTargetFolder = this.saveTargetFolderCheckBox.Checked;
		}

		/// <summary>
		/// Handles the Click event of the BrowseButton control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void BrowseButton_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				this.remapSettings.TargetFolder = dialog.SelectedPath;
				this.UpdateUI(this.remapSettings);
			}
		}

		/// <summary>
		/// Handles the CheckedChanged event of the saveTargetFolderCheckBox control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void saveTargetFolderCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			this.outputFolderBox.Enabled = !this.saveTargetFolderCheckBox.Checked;
			this.browseButton.Enabled = !this.saveTargetFolderCheckBox.Checked;
		}

		#endregion
	}
}