using System.Linq;
using Sdl.FileTypeSupport.Filters.Xliff;
using Sdl.FileTypeSupport.Framework.Integration;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Xunit;

namespace XliffXsltPreviewSample.Tests
{
	public class XliffXsltPreviewAdapterTests
	{
		[Fact]
		public void BuildPreviewSetsFactory_KeepsAltTransAndAddsXsltPreviewInStudioPane()
		{
			var sets = AdapterOverXliff().BuildPreviewSetsFactory(null).GetPreviewSets(null);

			Assert.Equal(new[] { "InternalPreview", "XsltPreview" }, sets.Select(s => s.Id.Id).ToArray());
			var target = Assert.IsAssignableFrom<IControlPreviewType>(sets.FindFromId(new PreviewSetId("XsltPreview")).Target);
			Assert.Equal("InternalPreview", target.SingleFilePreviewControlId.Value.Id);
			Assert.Equal("XsltPreview", target.TargetGeneratorId.Id);
		}

		[Fact]
		public void BuildAbstractGenerator_XsltPreviewIsXliffWriterWithMarkerAndTweaker_OthersGoToOriginal()
		{
			var adapter = AdapterOverXliff();

			var xslt = Assert.IsAssignableFrom<IFileGenerator>(adapter.BuildAbstractGenerator("Generator_XsltPreview"));
			var altTrans = Assert.IsAssignableFrom<IFileGenerator>(adapter.BuildAbstractGenerator("Generator_InternalPreview"));

			Assert.Equal("Sdl.FileTypeSupport.Filters.Xliff.Writer", xslt.BilingualWriter?.GetType().FullName);
			Assert.Contains(xslt.GetBilingualProcessors(), p => p is SegmentMarker);
			Assert.IsType<XsltPreviewTweaker>(Assert.Single(xslt.FileTweakers));
			Assert.DoesNotContain(altTrans.GetBilingualProcessors(), p => p is SegmentMarker);
			Assert.Empty(altTrans.FileTweakers);
		}

		private static XliffXsltPreviewAdapter AdapterOverXliff()
		{
			var manager = new PocoFilterManager(false);
			return new XliffXsltPreviewAdapter
			{
				FileTypeManager = manager,
				Original = new XliffComponentBuilder { FileTypeManager = manager }
			};
		}
	}
}
