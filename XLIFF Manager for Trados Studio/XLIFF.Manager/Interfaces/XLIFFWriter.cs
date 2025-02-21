using Sdl.Community.XLIFF.Manager.FileTypeSupport.XLIFF.Model;

namespace Sdl.Community.XLIFF.Manager.Interfaces
{
	public interface IXliffWriter
	{
		bool WriteFile(Xliff xliff, string outputFilePath, bool includeTranslations);
	}
}
