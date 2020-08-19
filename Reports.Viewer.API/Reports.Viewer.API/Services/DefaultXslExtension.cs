using System.IO;
using System.Reflection;
using System.Resources;

namespace Sdl.Reports.Viewer.API.Services
{
	public class DefaultXslExtension
	{
		private readonly Assembly _resourceAssembly;
		private ResourceManager _resourceManager;
		private readonly object _lockObject = new object();

		public DefaultXslExtension(Assembly resourceAssembly)
		{
			_resourceAssembly = resourceAssembly;
		}

		public string GetResourcesPath()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var location = assembly.Location;
			var directoryName = Path.GetDirectoryName(location);
			return directoryName == null ? null : Path.Combine(directoryName, "ReportResources");
		}

		public string GetImagesUrl()
		{
			return "file:///" + Path.Combine(GetResourcesPath(), "images");
		}

		public string GetFlagUrl(int lcid)
		{
			return GetImagesUrl() + "/flags/" + lcid + ".bmp";
		}

		public string GetDefaultCssLinkTag()
		{
			var linkFormat = "<LINK href=\"{0}\" type=\"text/css\" rel=\"stylesheet\">";
			return string.Format(linkFormat, "file:///" + Path.Combine(GetCssPath(), "reports.css"));
		}

		///// <summary>
		///// Generates the CSS styles for the TQA report elements (Added, Deleted, Comment)
		///// 
		///// Note: These styles are built into the Feedback.xsl file from Sdl.ProjectApi.AutomaticTasks.Feedback
		///// </summary>
		///// <returns></returns>
		//public string GetTQACssStyles()
		//{

		//	var settingsGroup = GlobalServices.UserSettingsService.UserSettings.GetSettingsGroup("EditorEditControlDisplaySettingsGroup");
		//	var formatting = new TQAFormatting(settingsGroup.SettingsBundle);


		//	var TQA_cssFormatting = "<style type=\"text/css\">"
		//		+ "  span.FeedbackAdded" + "{ " + formatting.CSSTQAAddedFormatting + " }"
		//		+ "  span.FeedbackDeleted" + "{ " + formatting.CSSTQADeletedFormatting + " }"
		//		+ "  span.FeedbackComment" + "{ " + formatting.CSSTQACommentFormatting + " }"
		//		+ "</style>";
		//	return TQA_cssFormatting;
		//}


		private string GetCssPath()
		{
			return Path.Combine(GetResourcesPath(), "css");
		}

		public string GetResourceString(string name)
		{
			try
			{
				return ResourceManager.GetString(name);
			}
			catch
			{
				// catch all; ignore
			}

			return name;
		}

		private ResourceManager ResourceManager
		{
			get
			{
				lock (_lockObject)
				{
					if (_resourceManager == null)
					{
						_resourceManager = new ResourceManager(_resourceAssembly.GetName().Name + ".StringResources", _resourceAssembly);
					}
				}

				return _resourceManager;
			}
		}
	}
}
