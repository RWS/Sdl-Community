namespace InterpretBank.TermSearch
{
	public interface IOpenFileDialog
	{
		string GetFilePath(string filter = null);
	}
}