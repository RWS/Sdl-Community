using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi.QuickInserts;

namespace Multilingual.XML.FileType.Models
{
	public class TagMap
	{
		public IQuickTag QuickTag { get; set; }

		public TagPair TagPair { get; set; }

		public QuickTagDefaultId QuickId { get; set; }

		public QuickInsertIds QuickInsertId { get; set; }
	}
}
