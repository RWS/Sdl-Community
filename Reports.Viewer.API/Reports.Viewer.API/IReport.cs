using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Sdl.Reports.Viewer.API
{
	public interface IReport : ICloneable
	{
		string Id { get; set; }

		string Name { get; set; }

		string Group { get; set; }

		string Language { get; set; }

		string Description { get; set; }

		string Path { get; set; }

		string Xslt { get; set; }

		DateTime Date { get; set; }

		[XmlIgnore]
		[JsonIgnore]
		string DateToString { get; }

		[XmlIgnore]
		[JsonIgnore]
		string DateToShortString { get; }

		bool IsExpanded { get; set; }

		bool IsSelected { get; set; }	
	}
}
