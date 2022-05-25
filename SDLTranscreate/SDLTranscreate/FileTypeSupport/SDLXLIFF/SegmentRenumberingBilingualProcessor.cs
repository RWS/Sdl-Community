using System.Globalization;
using System.Linq;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.FileTypeSupport.SDLXLIFF
{
	public class SegmentRenumberingBilingualProcessor : AbstractBilingualContentProcessor
	{
		long _nextSegmentId = 1;
		bool _enabled = false;

		/// <summary>
		/// When <c>false</c> (default) the processor does not re-number segments.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			set { _enabled = value; }
		}

		/// <summary>
		/// Integer number that will be assigned to the next encountered segment, if 
		/// the processor is <see cref="Enabled"/>.
		/// </summary>
		public long NextSegmentId
		{
			get { return _nextSegmentId; }
			set { _nextSegmentId = value; }
		}


		/// <summary>
		/// Overridden to determine if this is a file where segments should be re-numbered.
		/// </summary>
		/// <param name="fileInfo"></param>
		/// <remarks>
		/// <para>
		/// Sets the <see cref="Enabled"/> property to true or false depending on whether the
		/// file ID is part of a collection of file IDs in a published shared object of type <see cref="ISegmentRenumberingController"/>.
		/// </para>
		/// <para>
		/// To ensure that segment renumbering happens only once after segmentation the NeedsSegmentRenumbering flag is set to false.
		/// </para>
		/// </remarks>
		public override void SetFileProperties(IFileProperties fileInfo)
		{
			Enabled = (fileInfo.FileConversionProperties.GetMetaData(Constants.File_NeedsSegmentRenumbering) == true.ToString());
			if (Enabled)
			{
				fileInfo.FileConversionProperties.SetMetaData(Constants.File_NeedsSegmentRenumbering, false.ToString());
			}

			base.SetFileProperties(fileInfo);
		}


		/// <summary>
		/// Overridden to re-number segments when the processor is enabled.
		/// </summary>
		/// <param name="paragraphUnit"></param>
		public override void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			if (Enabled)
			{
				RenumberSegments(paragraphUnit);
			}
			else
			{
				// to ensure that multiple files parsed together get segments
				// numbered sequentially across files we track the last segment number
				// so that we can start re-numbering from that point if the processor is enabled.
				TrackLastSegmentNumber(paragraphUnit);
			}

			base.ProcessParagraphUnit(paragraphUnit);
		}


		private void TrackLastSegmentNumber(IParagraphUnit paragraphUnit)
		{
			var lastSegmentPair = paragraphUnit.SegmentPairs.LastOrDefault();
			if (lastSegmentPair != null)
			{
				long id;
				if (long.TryParse(lastSegmentPair.Properties.Id.Id, NumberStyles.Integer, CultureInfo.InvariantCulture, out id))
				{
					_nextSegmentId = id + 1;
				}
				else
				{
					// not an integer - fallback: increment by the number of segments in the paragraph
					_nextSegmentId += paragraphUnit.SegmentPairs.Count();
				}
			}
		}

		private void RenumberSegments(IParagraphUnit paragraphUnit)
		{
			foreach (var segPair in paragraphUnit.SegmentPairs)
			{
				segPair.Properties.Id = new SegmentId(_nextSegmentId.ToString(CultureInfo.InvariantCulture));
				++_nextSegmentId;
			}
		}
	}
}
