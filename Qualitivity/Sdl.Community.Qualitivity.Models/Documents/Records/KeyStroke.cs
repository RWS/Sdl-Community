using System;
using System.Xml.Serialization;

namespace Sdl.Community.Structures.Documents.Records
{
    [Serializable]
    public class KeyStroke : ICloneable
    {
		public KeyStroke()
		{
			Id = Guid.NewGuid().ToString();
			RecordId = -1;
			DocumentActivityId = -1;

			Created = null;
			OriginType = string.Empty;
			OriginSystem = string.Empty;
			Match = string.Empty;
			Selection = string.Empty;
			Key = string.Empty;
			Text = string.Empty;
			Ctrl = false;
			Alt = false;
			Shift = false;
			Position = -1;
			X = -1;
			Y = -1;
		}

		public string Id { get; set; }
        [XmlIgnore]
        public int RecordId { get; set; }
        [XmlIgnore]
        public int DocumentActivityId { get; set; }

        [XmlIgnore]
        public DateTime? Created { get; set; }
        [XmlAttribute(AttributeName = "created")]
        public string XmlCreated
        {
            get
            {
                return Created == null ? string.Empty : Helper.DateTimeToSqLite(Created.Value);

            }
            set
            {

                Created = value.Length == 0 ? null : Helper.DateTimeFromSqLite(value);

            }
        }

        public string OriginType { get; set; }

        public string OriginSystem { get; set; }

        public string Match { get; set; }

        public string Selection { get; set; }

        public string Key { get; set; }

        public string Text { get; set; }

        public bool Ctrl { get; set; }

        public bool Alt { get; set; }

        public bool Shift { get; set; }

	    public int Position { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

        public object Clone()
        {
            var keyStroke = new KeyStroke
            {
                Id = Id,
                RecordId = RecordId,
                DocumentActivityId = DocumentActivityId,
                Created = Created,
                OriginType = OriginType,
                OriginSystem = OriginSystem,
                Match = Match,
                Selection = Selection,
                Key = Key,
                Text = Text,
                Ctrl = Ctrl,
                Alt = Alt,
                Shift = Shift,
				Position = Position,
				X = X,
				Y = Y
            };

            return keyStroke;
        }
    }
}
