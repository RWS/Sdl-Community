using System;
using System.Windows.Forms;
using Sdl.Community.SDLBatchAnonymize.BatchTask;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.SDLBatchAnonymize.Ui
{
	public partial class BatchAnonymizerSettingsControl : UserControl,ISettingsAware<BatchAnonymizerSettings>
	{
		public BatchAnonymizerSettings Settings { get; set; }

		public bool AnonymizeComplete
		{
			get => completeBtn.Checked;
			set => completeBtn.Checked = value;
		}
		public bool AnonymizeTmMatch
		{
			get => tmMatchBtn.Checked;
			set => tmMatchBtn.Checked = value;
		}

		public decimal Score
		{
			get => scoreBox.Value;
			set => scoreBox.Value = value;
		}

		public string TmName
		{
			get => tmNameBox.Text;
			set => tmNameBox.Text = value;
		}

		public BatchAnonymizerSettingsControl()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			//create tooltips for buttons
			var completeBtnTooltip = new ToolTip();
			completeBtnTooltip.SetToolTip(completeBtn, "Simple anonymization");

			var tmMatchTooltip = new ToolTip();
			tmMatchTooltip.SetToolTip(tmMatchBtn,"Set an edited translation score and TM name");
			SetSettings(Settings);

			base.OnLoad(e);
		}

		private void SetSettings(BatchAnonymizerSettings settings)
		{
			completeBtn.Checked = settings.AnonymizeComplete;
			tmMatchBtn.Checked = settings.AnonymizeTmMatch;
			scoreBox.Value = settings.FuzzyScore;
			tmNameBox.Text = settings.TmName;
		}

	}
}
