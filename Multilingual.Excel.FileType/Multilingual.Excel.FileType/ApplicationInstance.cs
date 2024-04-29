using System.Windows.Forms;
using Multilingual.Excel.FileType.FileType.Processors;
using Multilingual.Excel.FileType.Services.Entities;
using Multilingual.Excel.FileType.Services;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Multilingual.Excel.FileType
{
	[ApplicationInitializer]
	internal class ApplicationInstance : IApplicationInitializer
	{
		private EditorController _controller;

		public void Execute()
		{
		}

		//public EditorController EditorController =>
		//	_controller ??= SdlTradosStudio.Application.GetController<EditorController>();

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
