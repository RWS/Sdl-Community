using System.Drawing;
using System.Runtime.Serialization;
using Sdl.Community.BeGlobalV4.Provider.ViewModel;

namespace Sdl.Community.BeGlobalV4.Provider.Model
{
	[DataContract]
	public class LangMappingMTCode : BaseViewModel
	{
		private string _codeName { get; set; }
		private Image _flag { get; set; }
		
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