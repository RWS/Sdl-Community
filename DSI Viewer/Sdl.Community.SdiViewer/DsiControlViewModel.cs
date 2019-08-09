using System;
using System.Collections.ObjectModel;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DsiViewer
{
	public class DsiControlViewModel:ModelBase
	{
		private ObservableCollection<DsiModel> _propertiesCollection;
		private readonly EditorController _editorController;
		public DsiControlViewModel()
		{
			_propertiesCollection = new ObservableCollection<DsiModel>();
			_editorController = GetEditorController();
			_editorController.Opened += EditorController_Opened;
		}

		private void EditorController_Opened(object sender, DocumentEventArgs e)
		{
			var activeDoc = _editorController.ActiveDocument;
			if (activeDoc != null)
			{
				AddProperties();
				activeDoc.ActiveSegmentChanged += ActiveDocument_ActiveSegmentChanged;
			}
		}

		private void ActiveDocument_ActiveSegmentChanged(object sender, EventArgs e)
		{
			AddProperties();
		}

		private void AddProperties()
		{
			PropertiesCollection.Clear();
			var doc = _editorController.ActiveDocument;
			var segment = doc?.ActiveSegmentPair;
			var contexts = segment?.GetParagraphUnitProperties().Contexts;
			if (contexts?.Contexts?.Count > 0)
			{
				foreach (var context in contexts.Contexts)
				{
					var color = context.DisplayColor;

					var sdiModel = new DsiModel
					{
						DisplayName = context.DisplayName,
						Description = context.Description,
						Code = context.DisplayCode,
					};
					if (color.Name == "0") // it doesn't have a color set
					{
						sdiModel.RowColor = "White";
					}
					else
					{
						sdiModel.RowColor = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
					}

					PropertiesCollection.Add(sdiModel);

				}
			}
		}

		public ObservableCollection<DsiModel> PropertiesCollection
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

