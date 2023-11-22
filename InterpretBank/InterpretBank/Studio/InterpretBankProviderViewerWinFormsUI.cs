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
				throw new NotImplementedException();
			}
		}

		public bool Initialized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public IEntry SelectedTerm
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

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
			throw new NotImplementedException();
		}

		public void JumpToTerm(IEntry entry)
		{
			throw new NotImplementedException();
		}

		public void Release()
		{
			throw new NotImplementedException();
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			throw new NotImplementedException();
		}
	}
}