using Sdl.Community.Utilities.TMTool.Task;
using Sdl.LanguagePlatform.TranslationMemory;
using System.Windows.Forms;

namespace Sdl.Community.Utilities.TMTool.Tasks.RemoveDuplicates
{
	public partial class RemoveDupControl : UserControl, IControl
	{
		private RemoveDupSettings _settings;

		/// <summary>
		/// initializes new RemoveDupControl
		/// </summary>
		public RemoveDupControl()
		{
			InitializeComponent();

			_settings = new RemoveDupSettings();
			ResetUI();
		}

		/// <summary>
		/// settings user control
		/// </summary>
		public UserControl UControl
		{
			get { return this; }
		}
		/// <summary>
		/// current settings
		/// </summary>
		public ISettings Options
		{
			get
			{
				GetSettings();
				return (ISettings)_settings;
			}
		}

		/// <summary>
		/// reset UI view, set default Options values
		/// </summary>
		public void ResetUI()
		{
			_settings.ResetToDefaults();
			UpdateUI(_settings);
		}
		/// <summary>
		/// updates UI with settings
		/// </summary>
		/// <param name="settings">settings to update to</param>
		public void UpdateUI(ISettings settings)
		{
			if (settings != null && settings.GetType() == _settings.GetType())
			{
				_settings = (RemoveDupSettings)settings;
				chBackup.Checked = _settings.IsBackupFiles;
				chPreservePsw.Checked = _settings.IsPreservePsw;
				switch (_settings.Scenario)
				{
					case ImportSettings.ImportTUProcessingMode.ProcessRawTUOnly:
						rbPresegmentedSc.Checked = true;
						break;
					case ImportSettings.ImportTUProcessingMode.ProcessBothTUs:
						rbMixedSc.Checked = true;
						break;
					default:
						rbDefaultSc.Checked = true;
						break;
				}
			}
		}

		private void GetSettings()
		{
			_settings.IsBackupFiles = chBackup.Checked;
			_settings.IsPreservePsw = chPreservePsw.Checked;

			_settings.Scenario = ImportSettings.ImportTUProcessingMode.ProcessCleanedTUOnly;
			if (rbPresegmentedSc.Checked)
				_settings.Scenario = ImportSettings.ImportTUProcessingMode.ProcessRawTUOnly;
			else if (rbMixedSc.Checked)
				_settings.Scenario = ImportSettings.ImportTUProcessingMode.ProcessBothTUs;
		}
	}
}