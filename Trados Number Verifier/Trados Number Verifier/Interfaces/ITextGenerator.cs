using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.NumberVerifier.Interfaces
{
	public interface ITextGenerator
	{
		string GetPlainText(ISegment segment, bool includeTagText);
	}
}
