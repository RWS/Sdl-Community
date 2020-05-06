using System.Collections.Generic;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ClientModel: BaseModel
	{
		//private bool _isSelected;
		//private bool _isExpanded;

		public ClientModel()
		{
			//_isExpanded = true;
		}

		public string Name { get; set; }

		public List<ProjectModel> ProjectModels { get; set; }

		//public bool IsSelected
		//{
		//	get => _isSelected;
		//	set
		//	{
		//		if (value != _isSelected)
		//		{
		//			_isSelected = value;
		//			OnPropertyChanged(nameof(IsSelected));					
		//		}
		//	}
		//}

		//public bool IsExpanded
		//{
		//	get => _isExpanded;
		//	set
		//	{
		//		if (value != _isExpanded)
		//		{
		//			_isExpanded = value;
		//			OnPropertyChanged(nameof(IsExpanded));
		//		}
		//	}
		//}
	}
}
