﻿using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Service;
using Sdl.Community.IATETerminologyProvider.View;
using Sdl.Terminology.TerminologyProvider.Core;

namespace Sdl.Community.IATETerminologyProvider
{
	[TerminologyProviderViewerWinFormsUI]
	internal class IATETerminologyProviderViewerWinFormsUI : ITerminologyProviderViewerWinFormsUI
	{
		private IATETerminologyProvider _iateTerminologyProvider;
		private IATETermsControl _control;
		private DocumentStateService _documentStateService;

		public event EventHandler TermChanged;
		public event EventHandler<EntryEventArgs> SelectedTermChanged;
		public event Action<IEntry> JumpToTermAction;
		public event Action<string, string> AddTermAction;

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

				if (_documentStateService == null)
				{
					_documentStateService = new DocumentStateService();
				}

				_documentStateService.UpdateDocumentEntriesState(_control);

				return _control;
			}
		}				

		public bool Initialized => true;

		public IEntry SelectedTerm { get; set; }

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
			if (JumpToTermAction != null)
			{
				JumpToTermAction -= _control.JumpToTerm;
			}

			_documentStateService.SaveDocumentEntriesState(_control);

			_control?.ReleaseSubscribers();
		}		

		public bool SupportsTerminologyProviderUri(Uri terminologyProviderUri)
		{
			return terminologyProviderUri.Scheme == Constants.IATEGlossary;
		}	
	}
}