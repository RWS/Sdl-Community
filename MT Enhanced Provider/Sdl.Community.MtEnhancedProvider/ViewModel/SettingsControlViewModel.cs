using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Sdl.Community.MtEnhancedProvider.Commands;
using Sdl.Community.MtEnhancedProvider.Model;
using Sdl.Community.MtEnhancedProvider.Model.Interface;
using Sdl.Community.MtEnhancedProvider.ViewModel.Interface;

namespace Sdl.Community.MtEnhancedProvider.ViewModel
{
	public class SettingsControlViewModel: ModelBase, ISettingsControlViewModel
	{
		private bool _reSendDraft;
		private bool _sendPlainText;
		private bool _doPreLookup;
		private bool _doPostLookup;
		private string _preLookupFileName;
		private string _postLookupFileName;

		public SettingsControlViewModel(IMtTranslationOptions options)
		{
			ViewModel = this;
			_options = options;
			BrowseCommand = new RelayCommand(Browse);
		}

		public ModelBase ViewModel { get; set; }
		public ICommand ShowMainWindowCommand { get; set; }
		public ICommand BrowseCommand { get; set; }

		public bool ReSendDraft
		{
			get => _reSendDraft;
			set
			{
				if (_reSendDraft == value) return;
				_reSendDraft = value;
				OnPropertyChanged(nameof(ReSendDraft));
			}
		}

		public bool SendPlainText
		{
			get => _sendPlainText;
			set
			{
				if (_sendPlainText == value) return;
				_sendPlainText = value;
				OnPropertyChanged(nameof(SendPlainText));
			}
		}

		public bool DoPreLookup
		{
			get => _doPreLookup;
			set
			{
				if (_doPreLookup == value) return;
				_doPreLookup = value;
				OnPropertyChanged(nameof(DoPreLookup));
			}
		}

		public bool DoPostLookup
		{
			get => _doPostLookup;
			set
			{
				if (_doPostLookup == value) return;
				_doPostLookup = value;
				OnPropertyChanged(nameof(DoPostLookup));
			}
		}

		public string PreLookupFileName
		{
			get => _preLookupFileName;
			set
			{
				if (_preLookupFileName == value) return;
				_preLookupFileName = value;
				OnPropertyChanged(nameof(PreLookupFileName));
			}
		}

		public string PostLookupFileName
		{
			get => _postLookupFileName;
			set
			{
				if (_postLookupFileName == value) return;
				_postLookupFileName = value;
				OnPropertyChanged(nameof(PostLookupFileName));
			}
		}

		private readonly IMtTranslationOptions _options;
		private void Browse(object commandParameter)
		{
			if (!string.IsNullOrEmpty(commandParameter.ToString()))
			{
				if (commandParameter.Equals(PluginResources.PreLookBrowse))
				{
					
				}
				if (commandParameter.Equals(PluginResources.PostLookupBrowse))
				{
					
				}
			}
		}
	}
}
