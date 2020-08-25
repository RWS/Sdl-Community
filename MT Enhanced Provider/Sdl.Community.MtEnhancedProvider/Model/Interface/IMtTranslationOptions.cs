using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.MtEnhancedProvider.Helpers;

namespace Sdl.Community.MtEnhancedProvider.Model.Interface
{
	public interface IMtTranslationOptions
	{
		MtTranslationOptions.ProviderType SelectedProvider { get; set; }
		string ApiKey { get; set; }
		bool UseCatID { get; set; }
		string CatId { get; set; }
		string JsonFilePath { get; set; }
		string ProjectName { get; set; }
		Enums.GoogleApiVersion SelectedGoogleVersion { get; set; }
	}
}
