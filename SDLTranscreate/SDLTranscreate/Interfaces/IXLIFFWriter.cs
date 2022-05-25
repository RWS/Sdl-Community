using Trados.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Trados.Transcreate.Interfaces
{
	public interface IXliffWriter
	{
		bool WriteFile(Xliff xliff, string outputFilePath, bool includeTranslations);
	}
}
