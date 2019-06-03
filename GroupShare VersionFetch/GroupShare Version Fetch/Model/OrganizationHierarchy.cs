using System.Collections.Generic;
using System.Linq;
using Sdl.Community.GSVersionFetch.Interface;

namespace Sdl.Community.GSVersionFetch.Model
{
	public class OrganizationHierarchy: BaseModel,ITreeViewItemModel
	{
		private bool _isSelected;
		private bool _isExpanded;

		public OrganizationHierarchy(string title,List<OrganizationHierarchy> children, OrganizationHierarchy parent=null)
		{
			Title = title;
			Parent = parent;
			Children = children;
		}

		public string Title { get; set; }
		public OrganizationHierarchy Parent { get; set; }
		public List<OrganizationHierarchy> Children { get; set; }
		public string SelectedValuePath => Title;

		public string DisplayValuePath => Title;


		public bool IsExpanded
		{
			get => _isExpanded;
			set
			{
				_isExpanded = value;
				OnPropertyChanged(nameof(IsExpanded));
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged(nameof(IsSelected));
			}
		}

		public IEnumerable<ITreeViewItemModel> GetHierarchy()
		{
			return GetAscendingHierarchy().Reverse();
		}

		public IEnumerable<ITreeViewItemModel> GetChildren()
		{
			return Children;
		}

		private IEnumerable<OrganizationHierarchy> GetAscendingHierarchy()
		{
			var vm = this;

			yield return vm;
			while (vm.Parent != null)
			{
				yield return vm.Parent;
				vm = vm.Parent;
			}
		}
	}
}
