namespace InterpretBank.Wrappers.Interface;

public interface IOpenFileDialog
{
	string GetFilePath(string filter = null);
}