using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Sdl.Community.Transcreate.Interfaces
{
	public interface IXliffReader
	{
		Xliff ReadXliff(string filePath);
	}
}
