using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Multilingual.XML.FileType.Common;
using Multilingual.XML.FileType.Models;
using Multilingual.XML.FileType.Providers;
using Rws.MultiSelectComboBox.API;

namespace Multilingual.XML.FileType.FileType.ViewModels
{
	public class AppendCommentMappingViewModel : INotifyPropertyChanged
	{
		private string _windowTitle;
		private string _commentPropertyName;

		private List<string> _commentPropertyTypes;
		private List<CommentProperty> _studioCommentProperties;

		private string _selectedCommentPropertyType;
		private CommentProperty _selectedStudioCommentProperty;

		private bool _isEditMode;
		private readonly List<CommentPropertyMapping> _commentMappings;
		private readonly StudioCommentPropertyProvider _studioCommentPropertyProvider;
		private readonly CommentPropertyMapping _commentPropertyMapping;


		public AppendCommentMappingViewModel(CommentPropertyMapping commentPropertyMapping,
			List<CommentPropertyMapping> commentMappings, StudioCommentPropertyProvider studioCommentPropertyProvider,
			IFilterService filterService, bool isEditMode)
		{
			_commentPropertyMapping = commentPropertyMapping ?? new CommentPropertyMapping();
			_commentMappings = commentMappings ?? new List<CommentPropertyMapping>();

			_studioCommentPropertyProvider = studioCommentPropertyProvider;
			IsEditMode = isEditMode;
			FilterService = filterService;

			CommentPropertyName = commentPropertyMapping?.PropertyName;

			var commentPropertyTypes = Enum.GetNames(typeof(Enumerators.CommentPropertyType)).ToList();
			//if (!IsEditMode && commentMappings != null && 
			//    commentMappings.Exists(a => a.PropertyType == Enumerators.CommentPropertyType.Text.ToString()))
			//{
			//	commentPropertyTypes.Remove(Enumerators.CommentPropertyType.Text.ToString());
			//}

			var commentProperties = new List<CommentProperty>();
			foreach (var commentProperty in _studioCommentPropertyProvider.DefaultCommentProperties)
			{
				if (!_commentMappings.Exists(a => a.StudioPropertyName == commentProperty.Name))
				{
					commentProperties.Add(commentProperty);
				}
				else if (isEditMode && commentProperty.Name == commentPropertyMapping?.StudioPropertyName)
				{
					commentProperties.Add(commentProperty);
				}
			}

			CommentPropertyTypes = commentPropertyTypes;
			StudioCommentProperties = commentProperties;

			if (commentPropertyMapping?.StudioPropertyName != null)
			{
				SelectedCommentPropertyType = commentPropertyMapping.PropertyType;
				SelectedStudioCommentProperty = StudioCommentProperties.FirstOrDefault(a => a.Name == commentPropertyMapping.StudioPropertyName);
			}
			else
			{
				//if (commentMappings != null && !commentMappings.Exists(a => a.PropertyType == Enumerators.CommentPropertyType.Text.ToString()))
				//{
				//	SelectedCommentPropertyType = Enumerators.CommentPropertyType.Text.ToString();
				//	SelectedStudioCommentProperty = StudioCommentProperties.FirstOrDefault(a => a.Type == Enumerators.CommentPropertyType.Text);
				//}
				//else
				//{
					SelectedCommentPropertyType = Enumerators.CommentPropertyType.Attribute.ToString();
					SelectedStudioCommentProperty = StudioCommentProperties.FirstOrDefault(a => a.Type == Enumerators.CommentPropertyType.Attribute);
				//}
			}

			WindowTitle = IsEditMode ? PluginResources.WindowTitle_Edit_CommentProperty : PluginResources.WindowTitle_Add_CommentProperty;
		}

		public string WindowTitle
		{
			get => _windowTitle;
			set
			{
				_windowTitle = value;
				OnPropertyChanged(nameof(WindowTitle));
			}
		}

		public IFilterService FilterService { get; set; }

		public bool IsEditMode
		{
			get => _isEditMode;
			set
			{
				if (_isEditMode == value)
				{
					return;
				}

				_isEditMode = value;
				OnPropertyChanged(nameof(IsEditMode));
			}
		}

		public List<string> CommentPropertyTypes
		{
			get => _commentPropertyTypes;
			set
			{
				_commentPropertyTypes = value;
				OnPropertyChanged(nameof(CommentPropertyTypes));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public string SelectedCommentPropertyType
		{
			get => _selectedCommentPropertyType;
			set
			{
				if (_selectedCommentPropertyType == value)
				{
					return;
				}

				_selectedCommentPropertyType = value;

				//if (_selectedCommentPropertyType == Enumerators.CommentPropertyType.Text.ToString())
				//{
				//	CommentPropertyName = string.Empty;
				//	CommentPropertyNameIsVisible = false;

				//	SelectedStudioCommentProperty = StudioCommentProperties.FirstOrDefault(a => a.Type == Enumerators.CommentPropertyType.Text);
				//}
				//else
				//{
				//	CommentPropertyNameIsVisible = true;
				//}
				//OnPropertyChanged(nameof(CommentPropertyNameIsVisible));

				OnPropertyChanged(nameof(SelectedCommentPropertyType));
				OnPropertyChanged(nameof(IsValid));
				OnPropertyChanged(nameof(SelectedStudioCommentProperty));
			}
		}

		public List<CommentProperty> StudioCommentProperties
		{
			get => _studioCommentProperties;
			set
			{
				_studioCommentProperties = value;
				OnPropertyChanged(nameof(StudioCommentProperties));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public CommentProperty SelectedStudioCommentProperty
		{
			get => _selectedStudioCommentProperty;
			set
			{
				if (_selectedStudioCommentProperty == value)
				{
					return;
				}

				_selectedStudioCommentProperty = value;
				OnPropertyChanged(nameof(SelectedStudioCommentProperty));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		//public bool CommentPropertyNameIsVisible { get; set; }

		public string CommentPropertyName
		{
			get => _commentPropertyName;
			set
			{
				if (_commentPropertyName == value)
				{
					return;
				}

				_commentPropertyName = value;
				OnPropertyChanged(nameof(CommentPropertyName));
				OnPropertyChanged(nameof(IsValid));
			}
		}

		public bool IsValid
		{
			get
			{
				var studioPropertyName = SelectedStudioCommentProperty?.Name;
				if (string.IsNullOrEmpty(studioPropertyName) || string.IsNullOrEmpty(CommentPropertyName?.Trim()))
				{
					return false;
				}

				if (IsEditMode)
				{

					if (_commentPropertyMapping.StudioPropertyName != studioPropertyName)
					{
						if (_commentMappings.Exists(a => a.StudioPropertyName == studioPropertyName))
						{
							return false;
						}
					}

					if (_commentPropertyMapping.PropertyName != CommentPropertyName)
					{
						if (_commentMappings.Exists(a => a.PropertyName == CommentPropertyName))
						{
							return false;
						}
					}
				}
				else
				{
					if (_commentMappings.Exists(a => a.StudioPropertyName == studioPropertyName || a.PropertyName == CommentPropertyName))
					{
						return false;
					}
				}

				return true;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
