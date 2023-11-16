using System;
using System.Drawing;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Ui;
using Sdl.Core.Globalization;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator
{
	[TerminologyProviderViewerWinFormsUI]
	public class TerminologyProviderViewerWinFormsUIExcel : ITerminologyProviderViewerWinFormsUI
	{
		private TermsList _control;
		private TerminologyProviderExcel _terminologyProvider;

		public event EventHandler<EntryEventArgs> SelectedTermChanged;

		public event EventHandler TermChanged;

		public bool IsEditing { get; }

		public Control Control
		{
			get
			{
				_control = new TermsList(_terminologyProvider)
				{
					Text = @"TerminologyProviderViewerWinFormsUIExcel",
					BackColor = Color.White
				};

				return _control;
			}
		}

		public bool Initialized => true;

		public Entry SelectedTerm { get; set; }

		public void AddAndEditTerm(Entry term, string source, string target)
		{
			var dataGrid = new ExcelData
			{
				Term = target,
				Approved = null
			};

			_control?.AddAndEdit(term, dataGrid);
		}

		public void CancelTerm()
		{
			
		}

		public void SaveTerm()
		{
			
		}

		public void AddTerm(string source, string target)
		{
			_control?.AddTerm(source, target);
		}

		public void EditTerm(Entry term)
		{
		}

		public void Initialize(ITerminologyProvider terminologyProvider, CultureCode source, CultureCode target)
		{
			_terminologyProvider = (TerminologyProviderExcel)terminologyProvider;
			Task.Run(_terminologyProvider.LoadEntries);
		}

		public void JumpToTerm(Entry entry)
		{
			_control?.JumpToTerm(entry);
		}

		public void Release()
		{
			_control?.Dispose();
			_terminologyProvider?.Dispose();
		}

		public bool CanAddTerm => true;

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "excelglossary";
		}
	}
}