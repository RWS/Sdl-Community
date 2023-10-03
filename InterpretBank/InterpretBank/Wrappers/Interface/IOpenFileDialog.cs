namespace InterpretBank.Wrappers.Interface;

public interface IDialog
{
	bool Confirm(string message);
	string GetFilePath(string filter);
}