namespace Sdl.Community.StarTransit.Interface
{
	public interface IOpenFileDialogService
	{
		/// <summary>
		/// Opens File Dialog window
		/// </summary>
		/// <param name="filter">Filter for the types of files</param>
		/// <returns>Full path of the selected file</returns>
		string ShowDialog(string filter);
	}
}
