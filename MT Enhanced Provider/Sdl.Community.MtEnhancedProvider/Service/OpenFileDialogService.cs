using Microsoft.Win32;
using Sdl.Community.MtEnhancedProvider.Service.Interface;

namespace Sdl.Community.MtEnhancedProvider.Service
{
	public class OpenFileDialogService:IOpenFileDialogService
	{
		/// <summary>
		/// Open File Dialog window
		/// </summary>
		/// <param name="filter">Files extensions</param>
		public string ShowDialog(string filter)
		{
			var openFileDialog = new OpenFileDialog
			{
				Filter = filter,
				Multiselect = false
			};
			return openFileDialog.ShowDialog() != true ? string.Empty: openFileDialog.FileName;
		}
	}
}
