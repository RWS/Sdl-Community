using Sdl.Community.XLIFF.Manager.Converters.XLIFF.Model;

namespace Sdl.Community.XLIFF.Manager.Interfaces
{
	public interface IXliffWriter
	{
		bool CreateXliffFile(Xliff xliff, string outputFilePath, bool includeTranslations);
	}
}
