using System;
using System.Collections.Generic;
using LanguageWeaverProvider.Model.Interface;
using LanguageWeaverProvider.ViewModel;
using Newtonsoft.Json;

namespace LanguageWeaverProvider.Model.Options
{
	public class TranslationOptions : BaseViewModel, ITranslationOptions
	{
		PluginVersion _pluginVersion;

		public TranslationOptions(bool isNewInstance = false)
		{
			if (isNewInstance)
            {
                AssignId();
            }
		}

        public void AssignId()
        {
            Id = Guid.NewGuid().ToString();
        }

        [JsonIgnore]
		public AccessToken AccessToken { get; set; }

		[JsonIgnore]
		public CloudCredentials CloudCredentials { get; set; }

		[JsonIgnore]
		public EdgeCredentials EdgeCredentials { get; set; }

		public PluginVersion PluginVersion
		{
			get => _pluginVersion;
			set
			{
				_pluginVersion = value;
				OnPropertyChanged();
			}
		}

		public AuthenticationType AuthenticationType { get; set; }

		public string Id { get; set; }

		public string ProviderName => GetProviderName();

		public List<PairMapping> PairMappings { get; set; }

		public ProviderSettings ProviderSettings { get; set; }

		public Uri Uri { get; set; }

		public void UpdateUri(string uriString)
		{
			Uri = new Uri(uriString);
		}

		public void UpdateUri(Uri uri)
		{
			UpdateUri(uri.AbsoluteUri);
		}

		public void UpdateUri()
		{
			UpdateUri(PluginVersion);
		}

		public void UpdateUri(PluginVersion pluginVersion)
		{
			switch (pluginVersion)
			{
				case PluginVersion.LanguageWeaverCloud:
					UpdateUri(Constants.CloudFullScheme);
					break;
				case PluginVersion.LanguageWeaverEdge:
					UpdateUri(Constants.EdgeFullScheme);
					break;
				default:
					break;
			}
		}

		private string GetProviderName()
		{
			var providerNamePrefix = PluginVersion switch
			{
				PluginVersion.LanguageWeaverCloud => Constants.PluginNameCloud,
				PluginVersion.LanguageWeaverEdge => Constants.PluginNameEdge,
				_ => Constants.PluginName
			};

			return ProviderSettings?.UseCustomName == true
				 ? $"{providerNamePrefix} - {ProviderSettings.CustomName}"
				 : providerNamePrefix;
		}
	}
}