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
		bool UseCatID { get; set; }
		bool PersistGoogleKey { get; set; }
		bool PersistMicrosoftCreds { get; set; }

		string CatId { get; set; }
		string JsonFilePath { get; set; }
		string ProjectName { get; set; }
		string ApiKey { get; set; } //Microsoft Key

		string ClientId { get; set; } // Google key
		Enums.GoogleApiVersion SelectedGoogleVersion { get; set; }
	}
}
