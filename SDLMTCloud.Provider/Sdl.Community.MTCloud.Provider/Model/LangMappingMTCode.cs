using System.Drawing;
using System.Runtime.Serialization;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	[DataContract]
	public class LangMappingMTCode : BaseViewModel
	{
		private string _codeName;
		private bool _isLocale;
		private Image _flag;

		public LangMappingMTCode()
		{
			IsLocale = false;
		}
		
		[DataMember]
		public string CodeName
		{
			get => _codeName;
			set
			{
				_codeName = value;
				OnPropertyChanged(nameof(CodeName));
			}
		}

		[DataMember]
		public bool IsLocale
		{
			get => _isLocale;
			set
			{
				_isLocale = value;
				OnPropertyChanged(nameof(IsLocale));
			}
		}

		public Image Flag
		{
			get => _flag;
			set
			{
				_flag = value;
				OnPropertyChanged(nameof(Flag));
			}
		}
	}
}