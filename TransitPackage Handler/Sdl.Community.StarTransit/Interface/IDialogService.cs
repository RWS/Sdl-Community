namespace Sdl.Community.StarTransit.Interface
{
	public interface IDialogService
	{
		string ShowFolderDialog(string dialogTitle);
		string ShowFileDialog(string filter,string title);
	}
}
