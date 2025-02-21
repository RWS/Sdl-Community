using System;

namespace Multilingual.Excel.FileType.QuickTags.Styles
{
    internal class FontColor
    {
        public bool? Auto { get; set; }
        public string Rgb { get; set; }
        public uint? Theme { get; set; }
        public double? Tint { get; set; }
        public int? Indexed { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((FontColor)obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        private bool Equals(FontColor other)
        {
            return Auto == other.Auto &&
                   string.Equals(Rgb, other.Rgb) &&
                   Theme == other.Theme &&
                   Tint == other.Tint &&
                   Indexed == other.Indexed;
        }

        public void Clear()
        {
            Theme = null;
            Tint = null;
            Indexed = null;
            Auto = null;
        }
    }
}
