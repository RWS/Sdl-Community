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
		private IATETerminologyProvider _terminologyProvider;

		public Control Control
		{
			get
			{
				return null;
			}
		}

		public bool Initialized
		{
			get
			{
				return true;
			}
		}

		public IEntry SelectedTerm
		{
			get
			{
				return null;
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
		}

		public void AddTerm(string source, string target)
		{
		}

		public void EditTerm(IEntry term)
		{
		}

		public void Initialize(ITerminologyProvider terminologyProvider, CultureInfo source, CultureInfo target)
		{
			_terminologyProvider = (IATETerminologyProvider)terminologyProvider;
		}

		public void JumpToTerm(IEntry entry)
		{
		}

		public void Release()
		{
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == "IATEglossary";
		}
	}
}
