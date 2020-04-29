using System;
using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;

namespace ExportToExcel
{
    public partial class ExclusionsUI : UserControl
    {
        private GeneratorSettings _settings;

        public ExclusionsUI()
        {
            InitializeComponent();
        }


        protected override void OnLoad(EventArgs e)
        {
            cbl_ExcludedStatuses.ItemCheck += Cbl_ExcludedStatuses_ItemCheck;
        }

        /// <summary>
        /// Helper method used for data binding
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cbl_ExcludedStatuses_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var chkList = sender as CheckedListBox;

	        if (chkList?.Items[e.Index] is SegmentStatus segmentStatus)
			{
				_settings.SetSegmentStatus(segmentStatus.Status, e.NewValue == CheckState.Checked);
			}
               
        }

        /// <summary>
        /// Calls Automatic task method for data binding
        /// </summary>
        /// <param name="settings"></param>
        public void SetSettings(GeneratorSettings settings)
        {
            
            _settings = settings;

            SettingsBinder.DataBindSetting<bool>(rb_ExcludeStatus, "Checked", _settings,
                nameof(_settings.ExcludeStatus));
            SettingsBinder.DataBindSetting<bool>(rb_ExcludeCategory, "Checked", _settings,
                nameof(_settings.ExclusionTypeCategory));
            SettingsBinder.DataBindSetting<bool>(cb_DontExportExact, "Checked", _settings,
                nameof(_settings.DontExportExact));
            SettingsBinder.DataBindSetting<bool>(cb_DontExportContext, "Checked", _settings,
                nameof(_settings.DontExportContext));
            SettingsBinder.DataBindSetting<bool>(cb_DontExportFuzzy, "Checked", _settings,
                nameof(_settings.DontExportFuzzy));
            SettingsBinder.DataBindSetting<bool>(cb_DontExportNoMatch, "Checked", _settings,
                nameof(_settings.DontExportNoMatch));
            UpdateUi(settings);
        }

 
        /// <summary>
        /// Updates the Ui after user set the changes
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateUi(GeneratorSettings settings)
        {
            _settings = settings;
            switch (_settings.ExcludeExportType)
            {
                case GeneratorSettings.ExclusionType.Category:
                    rb_ExcludeCategory.Checked = true;
                    break;
                case GeneratorSettings.ExclusionType.Status:
                    rb_ExcludeStatus.Checked = true;
                    break;
				case GeneratorSettings.ExclusionType.Locked:
					excludeLockedBtn.Checked = true;
					break;
                default:
                    break;
            }
            cbl_ExcludedStatuses.Items.Clear();
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.ApprovedSignOff));
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.ApprovedTranslation));
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.Draft));
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.RejectedSignOff));
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.RejectedTranslation));
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.Translated));
            AddSegmentStatusItem(new SegmentStatus(Sdl.Core.Globalization.ConfirmationLevel.Unspecified));

            cb_DontExportContext.Checked = _settings.DontExportContext;
            cb_DontExportExact.Checked = _settings.DontExportExact;
            cb_DontExportFuzzy.Checked = _settings.DontExportFuzzy;
            cb_DontExportNoMatch.Checked = _settings.DontExportNoMatch;
        }

        private void AddSegmentStatusItem(SegmentStatus itemStatus)
        {
            var isEnabled = _settings.ExcludedStatuses.Contains(itemStatus.Status);
            cbl_ExcludedStatuses.Items.Add(itemStatus, isEnabled);
        }

        
        /// <summary>
        /// Updates the settings that user selected
        /// </summary>
        /// <param name="settings"></param>
        public void UpdateSettings(GeneratorSettings settings)
        {
            _settings = settings;
            _settings.DontExportContext = cb_DontExportContext.Checked;
            _settings.DontExportExact = cb_DontExportExact.Checked;
            _settings.DontExportFuzzy = cb_DontExportFuzzy.Checked;
            _settings.DontExportNoMatch = cb_DontExportNoMatch.Checked;

            _settings.ExcludedStatuses.Clear();
            foreach (SegmentStatus item in cbl_ExcludedStatuses.CheckedItems)
            {
                _settings.ExcludedStatuses.Add(item.Status);
            }

            if (rb_ExcludeStatus.Checked)
            {
                _settings.ExcludeExportType = GeneratorSettings.ExclusionType.Status;
            }
			if (rb_ExcludeCategory.Checked)
			{
				_settings.ExcludeExportType = GeneratorSettings.ExclusionType.Category;
			}
			if (excludeLockedBtn.Checked)
            {
				_settings.ExcludeExportType = GeneratorSettings.ExclusionType.Locked;
			}
        }

        private void cb_DontExportContext_CheckedChanged(object sender, EventArgs e)
        {
            _settings.DontExportContext = cb_DontExportContext.Checked;   
        }

        private void cb_DontExportExact_CheckedChanged(object sender, EventArgs e)
        {
            _settings.DontExportExact = cb_DontExportExact.Checked;
        }

        private void cb_DontExportFuzzy_CheckedChanged(object sender, EventArgs e)
        {
            _settings.DontExportFuzzy = cb_DontExportFuzzy.Checked;
        }

        private void cb_DontExportNoMatch_CheckedChanged(object sender, EventArgs e)
        {
            _settings.DontExportNoMatch = cb_DontExportNoMatch.Checked;
        }

        private void rb_ExcludeStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_ExcludeStatus.Checked)
            {
                cbl_ExcludedStatuses.Enabled = true;
				_settings.ExcludeExportType = GeneratorSettings.ExclusionType.Status;

			}
            else
            {
                cbl_ExcludedStatuses.Enabled = false;
				//excludeLockedBtn.Enabled = false;
            }
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

		private void excludeLockedBtn_CheckedChanged(object sender, EventArgs e)
		{
			if (excludeLockedBtn.Checked)
			{
				excludeLockedBtn.Enabled = true;
				_settings.ExcludeExportType = GeneratorSettings.ExclusionType.Locked;
			}
			else
			{
				cbl_ExcludedStatuses.Enabled = false;
				
			}
		}

		private void rb_ExcludeCategory_CheckedChanged(object sender, EventArgs e)
		{
			if (rb_ExcludeCategory.Checked)
			{
				rb_ExcludeCategory.Enabled = true;
				SwitchStatus_CategoryItems(true);
				_settings.ExcludeExportType = GeneratorSettings.ExclusionType.Category;
			}
			else
			{
				SwitchStatus_CategoryItems(false);
			}
		}

	    private void SwitchStatus_CategoryItems(bool status)
		{
			cb_DontExportContext.Enabled = status;
			cb_DontExportExact.Enabled = status;
			cb_DontExportFuzzy.Enabled = status;
			cb_DontExportNoMatch.Enabled = status;
		}
	}
}
