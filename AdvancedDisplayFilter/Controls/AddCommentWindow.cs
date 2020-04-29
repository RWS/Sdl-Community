using System.Collections.Generic;
using System.Windows.Forms;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.AdvancedDisplayFilter.Controls
{
	public partial class AddCommentWindow : Form
	{
		public string Comment { get; set; }
		public Severity SeverityLevel { get; set; }

		public AddCommentWindow()
		{
			var severityLvls = new Dictionary<Severity, string>
			{
				{Severity.Low, "For your information"},
				{Severity.Medium,"Warning"},
				{Severity.High,"Error" }
			};

			InitializeComponent();
			severityBox.DataSource = new BindingSource(severityLvls,null);
			severityBox.DisplayMember = "Value";
			severityBox.ValueMember = "Key";
		}

		private void okBtn_Click(object sender, System.EventArgs e)
		{
			Comment = commentTextBox.Text;
			var selectedLvl = (KeyValuePair<Severity, string>)severityBox.SelectedItem;
			SeverityLevel = selectedLvl.Key;
		}
	}
}
