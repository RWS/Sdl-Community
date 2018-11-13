using Sdl.Terminology.TerminologyProvider.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace IATETerminologyProvider
{
	[TerminologyProviderViewerWinFormsUI]
	class IATETerminologyProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
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

		public event EventHandler<EntryEventArgs> SelectedTermChanged;
		public event EventHandler TermChanged;

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
