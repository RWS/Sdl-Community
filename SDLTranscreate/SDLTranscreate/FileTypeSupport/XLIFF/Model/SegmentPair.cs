using System;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.NativeApi;
using Trados.Transcreate.FileTypeSupport.SDLXLIFF;

namespace Trados.Transcreate.FileTypeSupport.XLIFF.Model
{
	public class SegmentPair : ICloneable
	{
		private readonly SegmentBuilder _segmentBuilder;

		public SegmentPair(SegmentBuilder segmentBuilder)
		{
			_segmentBuilder = segmentBuilder;

			ConfirmationLevel = ConfirmationLevel.Unspecified;
			TranslationOrigin = segmentBuilder.CreateTranslationOrigin();
			Source = new Source();
			Target = new Target();
		}

		public string Id { get; set; }

		public Source Source { get; set; }

		public Target Target { get; set; }

		public bool IsLocked { get; set; }

		public ConfirmationLevel ConfirmationLevel { get; set; }

		public ITranslationOrigin TranslationOrigin { get; set; }


		public object Clone()
		{
			var segmentPair = new SegmentPair(_segmentBuilder)
			{
				Id = Id,
				IsLocked = IsLocked,
				ConfirmationLevel = (ConfirmationLevel)Enum.Parse(typeof(ConfirmationLevel), ConfirmationLevel.ToString(), true),
				TranslationOrigin = TranslationOrigin?.Clone() as ITranslationOrigin,
				Source = Source.Clone() as Source,
				Target = Target.Clone() as Target
			};

			return segmentPair;
		}
	}
}
