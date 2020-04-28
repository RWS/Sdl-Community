using System.Drawing;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Sdl.Community.MTCloud.Provider.ViewModel;

namespace Sdl.Community.MTCloud.Provider.Model
{
	[DataContract]
	public class MTCloudLanguage : BaseViewModel
	{
		private string _codeName;
		private bool _isLocale;
		private Image _flag;

		public MTCloudLanguage()
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

		[JsonIgnore]
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