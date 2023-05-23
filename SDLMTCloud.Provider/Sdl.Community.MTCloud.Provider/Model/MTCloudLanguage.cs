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
		private Image _flag;
		
		[DataMember]
		public string CodeName
		{
			get => _codeName;
			set
			{
				_codeName = value;
				OnPropertyChanged();
			}
		}

		[JsonIgnore]
		public Image Flag
		{
			get => _flag;
			set
			{
				_flag = value;
				OnPropertyChanged();
			}
		}
	}
}