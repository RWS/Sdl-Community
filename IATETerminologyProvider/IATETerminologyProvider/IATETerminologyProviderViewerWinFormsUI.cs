using System;
using System.Globalization;
using System.Windows.Forms;
using IATETerminologyProvider.Helpers;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider
{
	[TerminologyProviderViewerWinFormsUI]
	class IATETerminologyProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
		#region Private Fields
		private IATETerminologyProvider _terminologyProvider;
		#endregion

		#region Public Properties
		public Control Control { get; set; }
		public bool Initialized => true;
		public IEntry SelectedTerm { get; set; }
		public event EventHandler TermChanged;
		public event EventHandler<EntryEventArgs> SelectedTermChanged;
		public event Action<IEntry> JumpToTermAction;
		public event Action<string, string> AddTermAction;
		#endregion

		#region Public Methods
		public void AddAndEditTerm(IEntry term, string source, string target)
		{
		}

		public void AddTerm(string source, string target)
		{
			AddTermAction?.Invoke(source, target);
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
			JumpToTermAction?.Invoke(entry);
		}

		public void Release()
		{
			_terminologyProvider = null;
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}
		#endregion
	}
}