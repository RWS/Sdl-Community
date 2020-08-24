using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sdl.Community.MtEnhancedProvider.Model.Interface
{
	public interface IMtTranslationOptions
	{
		MtTranslationOptions.ProviderType SelectedProvider { get; set; }
		string ApiKey { get; set; }
	}
}
