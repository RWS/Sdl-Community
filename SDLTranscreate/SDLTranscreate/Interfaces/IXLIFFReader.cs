using Trados.Transcreate.FileTypeSupport.XLIFF.Model;

namespace Trados.Transcreate.Interfaces
{
	public interface IXliffReader
	{
		Xliff ReadXliff(string filePath);
	}
}
