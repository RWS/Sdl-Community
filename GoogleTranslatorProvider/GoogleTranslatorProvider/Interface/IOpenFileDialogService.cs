namespace GoogleTranslatorProvider.Interfaces
{
	public interface IOpenFileDialogService
	{
		string ShowDialog(string filter = null);
	}
}