using System.Windows.Forms;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.TQA
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		public void Execute()
		{
		}

		public static Form GetActiveForm()
		{
			var allForms = System.Windows.Forms.Application.OpenForms;
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
