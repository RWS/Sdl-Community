using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using IATETerminologyProvider.Helpers;
using IATETerminologyProvider.Ui;
using Sdl.Terminology.TerminologyProvider.Core;

namespace IATETerminologyProvider
{
	[TerminologyProviderViewerWinFormsUI]
	class IATETerminologyProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
		#region Private Fields
		private IATETerminologyProvider _iateTerminologyProvider;
		private IATETermsControl _control;
		#endregion

		#region Public Properties
		public Control Control
		{
			get
			{
				_control = new IATETermsControl(_iateTerminologyProvider)
				{
					Text = @"IATETerminologyProviderViewerWinFormsUI",
					BackColor = Color.White
				};

				JumpToTermAction += _control.JumpToTerm;
				//_iateTerminologyProvider.TermsLoaded += _control.SetTerms;

				return _control;
			}
		}

		public bool Initialized => true;
		public IEntry SelectedTerm { get; set; }		
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
			_iateTerminologyProvider = (IATETerminologyProvider)terminologyProvider;
		}

		public void JumpToTerm(IEntry entry)
		{
			JumpToTermAction?.Invoke(entry);
		}

		public void Release()
		{
			_iateTerminologyProvider = null;
		}

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}
		#endregion

		#region Events
		public event EventHandler TermChanged;
		public event EventHandler<EntryEventArgs> SelectedTermChanged;
		public event Action<IEntry> JumpToTermAction;
		public event Action<string, string> AddTermAction;
		#endregion
	}
}