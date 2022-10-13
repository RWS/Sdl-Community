using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	public class SegmentRenumberingTrigger : AbstractBilingualContentProcessor
	{
		public static string NeedsSegmentRenumbering { get; set; } = "NeedsSegmentRenumbering";

		/// <summary>
		/// Overridden to set this file as one for which segments should be re-numbered,
		/// if such a controller is available through the shared objects.
		/// </summary>
		/// <param name="fileInfo"></param>
		public override void SetFileProperties(IFileProperties fileInfo)
		{
			fileInfo.FileConversionProperties.SetMetaData(NeedsSegmentRenumbering, true.ToString());

			base.SetFileProperties(fileInfo);
		}
	}
}
