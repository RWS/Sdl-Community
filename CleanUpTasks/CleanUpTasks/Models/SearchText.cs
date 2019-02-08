using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using Sdl.Community.CleanUpTasks.Utilities;

namespace Sdl.Community.CleanUpTasks.Models
{
	public class SearchText : BindableBase
    {
        private bool caseSensitive = false;
        private bool embeddedTags = false;
        private bool strConv = false;
        private bool tagPair = false;
        private string text = string.Empty;
        private bool useRegex = false;
        private List<VbStrConv> vbStrConv = new List<VbStrConv>();
        private bool wholeWord = false;

        [XmlElement]
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        [XmlElement]
        public bool CaseSensitive
        {
            get { return caseSensitive; }
            set { SetProperty(ref caseSensitive, value); }
        }

        [XmlElement]
        public bool EmbeddedTags
        {
            get { return embeddedTags; }
            set { SetProperty(ref embeddedTags, value); }
        }

        [XmlElement]
        public bool StrConv
        {
            get { return strConv; }
            set { SetProperty(ref strConv, value); }
        }

        [XmlElement]
        public bool TagPair
        {
            get { return tagPair; }
            set { SetProperty(ref tagPair, value); }
        }

        [XmlElement]
        public bool UseRegex
        {
            get { return useRegex; }
            set { SetProperty(ref useRegex, value); }
        }

        [XmlElement]
        public List<VbStrConv> VbStrConv
        {
            get { return vbStrConv; }
            set { SetProperty(ref vbStrConv, value); }
        }

        [XmlElement]
        public bool WholeWord
        {
            get { return wholeWord; }
            set { SetProperty(ref wholeWord, value); }
        }

        [XmlAttribute("space", Namespace = "http://www.w3.org/XML/1998/namespace")]
        public string Space { get; set; } = "preserve";

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SearchText s = (SearchText)obj;

            return text == s.text &&
                   caseSensitive == s.caseSensitive &&
                   useRegex == s.useRegex &&
                   wholeWord == s.wholeWord &&
                   strConv == s.strConv &&
                   vbStrConv == s.vbStrConv;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + text.GetHashCode();
                hash = hash * 31 + caseSensitive.GetHashCode();
                hash = hash * 31 + useRegex.GetHashCode();
                hash = hash * 31 + wholeWord.GetHashCode();
                hash = hash * 31 + strConv.GetHashCode();
                hash = hash * 31 + vbStrConv.GetHashCode();

                return hash;
            }
        }
    }
}