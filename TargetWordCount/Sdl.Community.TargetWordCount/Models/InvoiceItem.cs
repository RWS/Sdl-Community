using System.Xml.Serialization;

namespace Sdl.Community.TargetWordCount.Models
{
	public enum RateType
	{
		Locked,
		PerfectMatch,
		ContextMatch,
		Repetitions,
		CrossFileRepetitions,
		OneHundred,
		NinetyFive,
		EightyFive,
		SeventyFive,
		Fifty,
		New,
		Total
	}

	public class InvoiceItem
	{
		public InvoiceItem()
		{
		}

		public InvoiceItem(RateType type, string rate)
		{
			Type = type;
			Rate = rate;
		}

		[XmlElement]
		public string Rate { get; set; }

		[XmlElement]
		public RateType Type { get; set; }
	}
}