using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.StudioViews.Model
{
	public class AlignmentDataType
	{
		public string Id { get; set; }

		public int IndexInParent { get; set; }

		public IAbstractMarkupData MarkupData { get; set; }
	}
}
