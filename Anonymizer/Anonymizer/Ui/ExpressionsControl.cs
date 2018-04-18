using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Sdl.Community.Anonymizer.Models;
using Sdl.Desktop.IntegrationApi;

namespace Sdl.Community.Anonymizer.Ui
{
	public partial class ExpressionsControl : UserControl
	{
		private AnonymizerSettings _settings;
	//	private BindingList<RegexPattern> _expressions;
		private List<RegexPattern> _expressions;
		public ExpressionsControl()
		{
			InitializeComponent();
			expressionsDataGrid.AutoGenerateColumns = false;

			//TO DO
		//	//read information from JSON 
		//	_expressions = new BindingList<RegexPattern>
		//{
		//	new RegexPattern
		//	{
		//		Description = "bla bla",
		//		Pattern = "daasda"
		//	},

		//	new RegexPattern
		//	{
		//		Description = "bla bla",
		//		Pattern = "daasda"
		//	},
		//	new RegexPattern
		//	{
		//		Description = "bla bla",
		//		Pattern = "daasda"
		//	}
		//};
		//	expressionsDataGrid.DataSource = _expressions;

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


		public void SetSettings(AnonymizerSettings settings)
		{
			_settings = settings;

			UpdateUi(settings);
		}

		private void UpdateUi(AnonymizerSettings settings)
		{
			_settings = settings;

			ReadExistingExpressions();
		}

		private void ReadExistingExpressions()
		{
			_expressions = new List<RegexPattern>
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
			expressionsDataGrid.DataSource = _expressions;
		}

		public void UpdateSettings(AnonymizerSettings settings)
		{
			_settings = settings;

			foreach (var expression in _expressions)
			{
				_settings.RegexPatterns.Add(expression);
			}
			//_settings.RegexPatterns.AddRange(_expressions)
		}

		private void expressionsDataGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 3 && e.RowIndex >= 0)
			{
				expressionsDataGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
				var exppression = _expressions[e.RowIndex];
				exppression.ShouldEncrypt = (bool) expressionsDataGrid.CurrentCell.Value;
			}
				
		}
	}
}
