using System;
using System.Xml.Serialization;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.Models
{
	[XmlType("Comment")]
	[Serializable]
	public class Comment: AbstractMetaDataContainer, IComment, ISupportsPersistenceId
	{
		private string _text;
		private string _user;
		private string _version;

		private DateTime _date;  // stores the modification time in UTC

		private Severity _severity;

		[NonSerialized]
		private int _persistenceId;

		// this will generate a value of 0 for the COM DATE data type:
		public static DateTime NoDateValue = new DateTime(1899, 12, 30);

		public Comment()
		{
			_text = "";
			_user = "";
			_version = "";
			_severity = Severity.Undefined;
			_date = NoDateValue;
		}

		public Comment(string user, string version, string text)
		{
			_text = text;
			_user = user;
			_version = version;
			_severity = Severity.Undefined;
		}

		public Comment(IComment other)
		{
			_text = other.Text;
			_user = other.Author;
			_version = other.Version;
			_date = other.Date;
			_severity = other.Severity;
			ReplaceMetaDataWithCloneOf(((AbstractMetaDataContainer)other).MetaData);
		}

		protected Comment(Comment other)
		{
			_text = other._text;
			_user = other._user;
			_version = other._version;
			_date = other._date;
			_severity = other._severity;
		}

		[XmlText]
		public string Text
		{
			get => _text;
			set => _text = value;
		}

		[XmlAttribute("user")]
		public string Author
		{
			get => _user;
			set => _user = value;
		}

		[XmlAttribute("version")]
		public string Version
		{
			get => _version;
			set => _version = value;
		}

		[XmlAttribute("date")]
		public DateTime Date
		{
			get => _date;
			set => _date = value;
		}

		[XmlIgnore]
		public bool DateSpecified
		{
			get => _date != NoDateValue;
			set
			{
			}
		}

		///// <summary>The severity level has not been specified</summary>
		//Undefined = 0,
		///// <summary>Informational purpose</summary>
		//Low = 1,
		///// <summary>Warning, likely an important issue</summary>
		//Medium = 2,
		///// <summary>Error, a severe issue</summary>
		//High = 3,
		///// <summary>Sentinel, not used</summary>
		//Invalid = 100, // 0x00000064
		[XmlAttribute("severity")]
		public Severity Severity
		{
			get => _severity;
			set => _severity = value;
		}

		[XmlIgnore]
		public bool SeveritySpecified => (uint)_severity > 0U;

		[XmlIgnore]
		public int PersistenceId
		{
			get => _persistenceId;
			set => _persistenceId = value;
		}

		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			var other = obj as Comment;

			if (_text != other?._text)
			{
				return false;
			}
			if (_user != other?._user)
			{
				return false;
			}
			if (_version != other?._version)
			{
				return false;
			}
			if (!_date.Equals(other?._date))
			{
				return false;
			}
			if (!_severity.Equals(other?._severity))
			{
				return false;
			}

			// all properties are equal -> object considered equal.
			return true;
		}

		public object Clone()
		{
			return new Comment(this);
		}
	}
}
