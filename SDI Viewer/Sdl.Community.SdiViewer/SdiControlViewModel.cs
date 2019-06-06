using System;
using System.Collections.ObjectModel;
using System.Linq;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.SdiViewer
{
	public class SdiControlViewModel : ModelBase
	{
		private  ObservableCollection<SdiModel> _propertiesCollection;
		private readonly EditorController _editorController;
		public SdiControlViewModel()
		{
			_propertiesCollection = new ObservableCollection<SdiModel>();
			 _editorController = GetEditorController();
			_editorController.Opened += EditorController_Opened;
		}

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			var activeDoc = _editorController.ActiveDocument;
			if (activeDoc != null)
			{
				activeDoc.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
				var activeFile = activeDoc.ActiveFile;
				_editorController.ActiveDocument.SetActiveSegmentPair(activeFile, "1");
			}
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			PropertiesCollection.Clear();
			var doc = sender as Document;
			var segment = doc?.ActiveSegmentPair;
			var contexts = segment?.GetParagraphUnitProperties().Contexts;
			if (contexts?.Contexts?.Count > 0)
			{
				foreach (var context in contexts.Contexts)
				{
					var sdiModel = new SdiModel
					{
						DisplayName = context.DisplayName,
						Description = context.Description,
						Code = context.DisplayCode
					};
					PropertiesCollection.Add(sdiModel);
				}
			}
		}

		public ObservableCollection<SdiModel> PropertiesCollection
		{
			get => _propertiesCollection;
			set
			{
				_propertiesCollection = value;
				OnPropertyChanged(nameof(PropertiesCollection));
			}
		}
		private EditorController GetEditorController()
		{
			return SdlTradosStudio.Application.GetController<EditorController>();
		}
	}
}
