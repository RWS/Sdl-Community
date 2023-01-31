using System;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Terminology.TerminologyProvider.Core;

namespace InterpretBank.Studio
{
	[TerminologyProviderViewerWinFormsUI]
	internal class InterpretBankProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
		public event EventHandler<EntryEventArgs> SelectedTermChanged;

		public event EventHandler TermChanged;

		public Control Control
		{
			get
			{
				return new Control();
			}
		}

		public bool Initialized
		{
			get
			{
				return true;
			}
		}

		public IEntry SelectedTerm { get; set; }

		public void AddAndEditTerm(IEntry term, string source, string target)
		{
			throw new NotImplementedException();
		}

		public void AddTerm(string source, string target)
		{
			throw new NotImplementedException();
		}

		public void EditTerm(IEntry term)
		{
			throw new NotImplementedException();
		}

		public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
		{
		}

		public void JumpToTerm(IEntry entry)
		{
			throw new NotImplementedException();
		}

		public void Release()
		{
			
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri == new Uri(Constants.InterpretBankUri);
		}
	}
}