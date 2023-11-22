using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Multilingual.XML.FileType
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		public void Execute()
		{
			// Setup Logger
		}

		public static Form GetActiveForm()
		{
			var allForms = Application.OpenForms;
			var activeForm = allForms[allForms.Count - 1];
			foreach (Form form in allForms)
			{
				if (form.GetType().Name == "StudioWindowForm")
				{
					activeForm = form;
					break;
				}
			}

			return activeForm;
		}
	}
}
