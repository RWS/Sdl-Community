using Sdl.Community.DeepLMTProvider.Model;

namespace Sdl.Community.DeepLMTProvider.Interface
{
	public interface ITsvReader
	{
		Glossary ReadTsvGlossary(string filename);
	}
}