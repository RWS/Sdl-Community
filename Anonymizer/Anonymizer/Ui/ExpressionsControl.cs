using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Community.Anonymizer.Models;

namespace Sdl.Community.Anonymizer.Ui
{
	public partial class ExpressionsControl : UserControl
	{
		public ExpressionsControl()
		{
			InitializeComponent();
			expressionsDataGrid.AutoGenerateColumns = false;

			//TO DO
			//read information from JSON 
			var expressions = new BindingList<RegexPattern>
		{
			new RegexPattern
			{
				Description = "bla bla",
				Pattern = "daasda"
			},

			new RegexPattern
			{
				Description = "bla bla",
				Pattern = "daasda"
			},
			new RegexPattern
			{
				Description = "bla bla",
				Pattern = "daasda"
			}
		};
			expressionsDataGrid.DataSource = expressions;

			//genteate columns
		
			
			var exportColumn = new DataGridViewCheckBoxColumn
			{
				HeaderText = @"Export?"
			};
			expressionsDataGrid.Columns.Add(exportColumn);
			var pattern = new DataGridViewTextBoxColumn
			{
				HeaderText = @"Regex Pattern",
				DataPropertyName = "Pattern"
			};
			expressionsDataGrid.Columns.Add(pattern);
			var description = new DataGridViewTextBoxColumn
			{
				HeaderText = @"Description",
				DataPropertyName = "Description"
			};
			expressionsDataGrid.Columns.Add(description);
			var shouldEncryptColumn = new DataGridViewCheckBoxColumn
			{
				HeaderText = @"Encrypt?",
				//Width = 60
			};
			expressionsDataGrid.Columns.Add(shouldEncryptColumn);
		}


	}
}
