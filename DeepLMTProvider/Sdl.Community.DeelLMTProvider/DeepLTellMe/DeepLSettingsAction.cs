using System.Linq;
using System.Xml;
using Sdl.Community.DeepLMTProvider.WPF;
using Sdl.Community.DeepLMTProvider.WPF.Model;
using Sdl.TellMe.ProviderApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.DeepLMTProvider.DeepLTellMe
{
	public class DeepLSettingsAction: AbstractTellMeAction
	{
		public DeepLSettingsAction()
		{
			Name = "DeepL provider settings for active project";
		}
		public override void Execute()
		{
			var currentProject = SdlTradosStudio.Application.GetController<ProjectsController>().CurrentProject;
			var settings = currentProject.GetTranslationProviderConfiguration();
			var settingsFile = currentProject.FilePath;
			var translationProvider = settings.Entries.FirstOrDefault(entry => entry.MainTranslationProvider.Uri.OriginalString.Contains("deepltranslationprovider:///"));
			if (translationProvider != null)
			{
				var uri = translationProvider.MainTranslationProvider.Uri;
				var options = new DeepLTranslationOptions(uri);
				var dialog = new DeepLWindow(options, true);
				dialog.ShowDialog();
				if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
				{
					var newOptionsUri = dialog.Options.Uri.ToString();
					if (!string.IsNullOrEmpty(newOptionsUri))
					{
						SaveNewSettings(settingsFile,newOptionsUri);
					}
				}
			}
		}

		public override bool IsAvailable => true;
		public override string Category => "DeepL results";

		private void SaveNewSettings(string filePath,string uri)
		{
			var xml = new XmlDocument();
			xml.Load(filePath);

			var cascadeItemNodes = xml.GetElementsByTagName("CascadeItem");
			foreach (XmlNode cascadeNode in cascadeItemNodes)
			{
				foreach (XmlNode child in cascadeNode.ChildNodes)
				{
					if (child.Name.Equals("CascadeEntryItem"))
					{
						var mainTranslation = child.ChildNodes.OfType<XmlElement>()
							.FirstOrDefault(t => t.Name.Equals("MainTranslationProviderItem"));
						if (mainTranslation != null)
						{
							foreach (XmlAttribute attribute in mainTranslation.Attributes)
							{
								if (attribute.Name.Equals("Uri") && attribute.Value.Contains("deepltranslationprovider:///"))
								{
									attribute.Value = uri;
								}
							}  
						}
					}
				}
			}
			xml.Save(filePath);
		}
	}
}
