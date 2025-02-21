using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface ITsvReaderWriter
	{
		Glossary ReadTsvGlossary(string filename);
	}
}