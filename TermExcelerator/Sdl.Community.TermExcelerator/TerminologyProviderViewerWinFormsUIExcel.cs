using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Community.TermExcelerator.Model;
using Sdl.Community.TermExcelerator.Ui;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.TermExcelerator
{
	[TerminologyProviderViewerWinFormsUI]
	public class TerminologyProviderViewerWinFormsUIExcel : ITerminologyProviderViewerWinFormsUI
	{
		private TerminologyProviderExcel _terminologyProvider;
		private TermsList _control;

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


		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "excelglossary";
		}

		public void JumpToTerm(IEntry entry)
		{
			_control?.JumpToTerm(entry);
		}

		public void AddTerm(string source, string target)
		{
			_control?.AddTerm(source, target);
		}

		public void EditTerm(IEntry term)
		{

		}

		public void AddAndEditTerm(IEntry term, string source, string target)
		{
			var dataGrid = new ExcelData
			{
				Term = target,
				Approved = null
			};

			_control?.AddAndEdit(term, dataGrid);
		}

		public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
		{
			_terminologyProvider = (TerminologyProviderExcel)terminologyProvider;
		}

		public void Release()
		{
			_terminologyProvider = null;
		}

		public bool Initialized => true;
		public IEntry SelectedTerm { get; set; }
		public event EventHandler TermChanged;
		public event EventHandler<EntryEventArgs> SelectedTermChanged;
	}
}
