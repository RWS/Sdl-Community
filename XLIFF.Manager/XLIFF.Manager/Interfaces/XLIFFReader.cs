using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;

namespace Sdl.Community.XLIFF.Manager.Interfaces
{
	public interface IXliffReader
	{
		Xliff ReadXliff(string filePath);
	}
}
