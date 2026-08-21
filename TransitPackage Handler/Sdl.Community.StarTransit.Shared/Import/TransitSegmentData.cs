using System;

namespace Sdl.Community.StarTransit.Shared.Import
{
	/// <summary>
	/// Reads the fields Transit packs into the Data attribute of a Seg node.
	/// The attribute holds a list of fields, each introduced by a marker character from the private use area.
	/// Most fields carry their value in the low byte of the marker itself (EB64 = match rate 100); the rest are
	/// followed by plain text running up to the next marker (EF02 = path of the file the pretranslation came from).
	/// </summary>
	public class TransitSegmentData
	{
		private const int MarkerRangeStart = 0xE000;
		private const int MarkerRangeEnd = 0xF8FF;
		private const int SegmentFlagsMarker = 0xEA;
		private const int MatchRateMarker = 0xEB;
		private const int ReferenceFileMarker = 0xEF02;
		private const int MachineTranslationFlag = 0x20;
		private const string MachineTranslationFilePrefix = "_AEXTR_MT_";

		private readonly string _data;

		public TransitSegmentData(string data)
		{
			_data = data ?? string.Empty;
		}

		/// <summary>
		/// Name of the file the pretranslation was taken from, without its folder.
		/// </summary>
		public string ReferenceFileName
		{
			get
			{
				var referenceFile = GetFieldValue(ReferenceFileMarker);
				return referenceFile.Substring(referenceFile.LastIndexOf('\\') + 1);
			}
		}

		/// <summary>
		/// True when the segment was pretranslated by a machine translation engine rather than from a
		/// translation memory or reference material.
		/// </summary>
		public bool IsMachineTranslated
		{
			get
			{
				var segmentFlags = GetMarkerValue(SegmentFlagsMarker);
				if (segmentFlags != null && (segmentFlags.Value & MachineTranslationFlag) == MachineTranslationFlag)
				{
					return true;
				}

				//Transit versions which do not set the flag are recognised by the machine translation
				//extract the pretranslation was taken from (_AEXTR_MT_<engine>.<language code>)
				return ReferenceFileName.StartsWith(MachineTranslationFilePrefix, StringComparison.OrdinalIgnoreCase);
			}
		}

		/// <summary>
		/// How closely the pretranslation matched the source, 0 when the segment was not pretranslated.
		/// Machine translation carries no match rate.
		/// </summary>
		public byte MatchRate
		{
			get
			{
				var matchRate = GetMarkerValue(MatchRateMarker);
				if (matchRate != null)
				{
					return (byte)Math.Min(matchRate.Value, 100);
				}

				//Transit versions which do not record a match rate only ever pretranslated with full matches,
				//recognisable by the path of the file the pretranslation was taken from
				return _data.Contains("\\") ? (byte)100 : (byte)0;
			}
		}

		/// <summary>
		/// Reads a field whose value is held in the low byte of its own marker, for example EB64 (match rate 100).
		/// </summary>
		/// <returns>The value of the field, or null when the segment does not carry it</returns>
		private int? GetMarkerValue(int markerPrefix)
		{
			foreach (var character in _data)
			{
				if (character >> 8 == markerPrefix)
				{
					return character & 0xFF;
				}
			}
			return null;
		}

		/// <summary>
		/// Reads a field whose value is the text between its marker and the marker of the following field,
		/// for example EF02 (path of the file the pretranslation was taken from).
		/// </summary>
		private string GetFieldValue(int marker)
		{
			var start = _data.IndexOf((char)marker) + 1;
			if (start == 0) return string.Empty;

			var end = start;
			while (end < _data.Length && !(_data[end] >= MarkerRangeStart && _data[end] <= MarkerRangeEnd))
			{
				end++;
			}
			return _data.Substring(start, end - start);
		}
	}
}
