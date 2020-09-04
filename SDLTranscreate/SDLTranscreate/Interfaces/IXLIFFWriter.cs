using Sdl.Community.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Sdl.Community.Transcreate.Interfaces
{
	public interface IXliffWriter
	{
		bool WriteFile(Xliff xliff, string outputFilePath, bool includeTranslations);
	}
}
