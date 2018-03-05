//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using Sdl.Community.AhkPlugin.Annotations;

//namespace Sdl.Community.AhkPlugin.ItemTemplates
//{
//    public class ScriptsDataGridItemTemplate : INotifyPropertyChanged
//    {
//	    private bool _isSelected;
//	    public string Name { get; set; }
//	    public string Description { get; set; }	
//		public bool IsSelected
//		{
//			get => _isSelected;
//			set
//			{
//				if (_isSelected != value)
//				{
//					_isSelected = value;
//					OnPropertyChanged(nameof(IsSelected));
//				}
//			}
//		}
//		public event PropertyChangedEventHandler PropertyChanged;

//		[NotifyPropertyChangedInvocator]
//		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
//		{
//			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//		}
//	}
//}
