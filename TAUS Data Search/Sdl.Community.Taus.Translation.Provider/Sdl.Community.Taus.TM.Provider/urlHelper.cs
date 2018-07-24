using System.Collections.Generic;

namespace Sdl.Community.Taus.Translation.Provider.Sdl.Community.Taus.TM.Provider
{
    class UrlHelper
    {
        public static Dictionary<string, string> EncodingChars;

        static UrlHelper()
        {
            EncodingChars = new Dictionary<string, string>();

            EncodingChars.Add(" ", "%20");
            EncodingChars.Add("!", "%21");
            EncodingChars.Add("\"", "%22");
            EncodingChars.Add("#", "%23");
            EncodingChars.Add("$", "%24");
            EncodingChars.Add("%", "%25");
            EncodingChars.Add("&", "%26");
            EncodingChars.Add("'", "%27");
            EncodingChars.Add("(", "%28");
            EncodingChars.Add(")", "%29");
            EncodingChars.Add("*", "%2A");
            EncodingChars.Add("+", "%2B");
            EncodingChars.Add(",", "%2C");
            EncodingChars.Add("-", "%2D");
            EncodingChars.Add(".", "%2E");
            EncodingChars.Add("/", "%2F");
            EncodingChars.Add(":", "%3A");
            EncodingChars.Add(";", "%3B");
            EncodingChars.Add("<", "%3C");
            EncodingChars.Add("=", "%3D");
            EncodingChars.Add(">", "%3E");
            EncodingChars.Add("?", "%3F");
            EncodingChars.Add("@", "%40");
            EncodingChars.Add("[", "%5B");
            EncodingChars.Add("\\", "%5C");
            EncodingChars.Add("]", "%5D");
            EncodingChars.Add("^", "%5E");
            EncodingChars.Add("_", "%5F");
            EncodingChars.Add("`", "%60");
            EncodingChars.Add("{", "%7B");
            EncodingChars.Add("|", "%7C");
            EncodingChars.Add("}", "%7D");
            EncodingChars.Add("~", "%7E");
            EncodingChars.Add(((char)127).ToString(), "%7F");
            EncodingChars.Add("€", "%80");
            EncodingChars.Add(((char)129).ToString(), "%81");
            EncodingChars.Add("‚", "%82");
            EncodingChars.Add("ƒ", "%83");
            EncodingChars.Add("„", "%84");
            EncodingChars.Add("…", "%85");
            EncodingChars.Add("†", "%86");
            EncodingChars.Add("‡", "%87");
            EncodingChars.Add("ˆ", "%88");
            EncodingChars.Add("‰", "%89");
            EncodingChars.Add("Š", "%8A");
            EncodingChars.Add("‹", "%8B");
            EncodingChars.Add("Œ", "%8C");
            EncodingChars.Add(((char)141).ToString(), "%8D");
            EncodingChars.Add("Ž", "%8E");
            EncodingChars.Add(((char)143).ToString(), "%8F");
            EncodingChars.Add(((char)144).ToString(), "%90");
            EncodingChars.Add("‘", "%91");
            EncodingChars.Add("’", "%92");
            EncodingChars.Add("“", "%93");
            EncodingChars.Add("”", "%94");
            EncodingChars.Add("•", "%95");
            EncodingChars.Add("–", "%96");
            EncodingChars.Add("—", "%97");
            EncodingChars.Add("˜", "%98");
            EncodingChars.Add("™", "%99");
            EncodingChars.Add("š", "%9A");
            EncodingChars.Add("›", "%9B");
            EncodingChars.Add("œ", "%9C");
            EncodingChars.Add(((char)157).ToString(), "%9D");
            EncodingChars.Add("ž", "%9E");
            EncodingChars.Add("Ÿ", "%9F");
            EncodingChars.Add(((char)112).ToString(), "%A0");
            EncodingChars.Add("¡", "%A1");
            EncodingChars.Add("¢", "%A2");
            EncodingChars.Add("£", "%A3");
            EncodingChars.Add(((char)164).ToString(), "%A4");
            EncodingChars.Add("¥", "%A5");
            EncodingChars.Add(((char)166).ToString(), "%A6");
            EncodingChars.Add("§", "%A7");
            EncodingChars.Add("¨", "%A8");
            EncodingChars.Add("©", "%A9");
            EncodingChars.Add("ª", "%AA");
            EncodingChars.Add("«", "%AB");
            EncodingChars.Add("¬", "%AC");
            EncodingChars.Add("¯", "%AD");
            EncodingChars.Add("®", "%AE");
            EncodingChars.Add("°", "%B0");
            EncodingChars.Add("±", "%B1");
            EncodingChars.Add("²", "%B2");
            EncodingChars.Add("³", "%B3");
            EncodingChars.Add("´", "%B4");
            EncodingChars.Add("µ", "%B5");
            EncodingChars.Add("¶", "%B6");
            EncodingChars.Add("·", "%B7");
            EncodingChars.Add("¸", "%B8");
            EncodingChars.Add("¹", "%B9");
            EncodingChars.Add("º", "%BA");
            EncodingChars.Add("»", "%BB");
            EncodingChars.Add("¼", "%BC");
            EncodingChars.Add("½", "%BD");
            EncodingChars.Add("¾", "%BE");
            EncodingChars.Add("¿", "%BF");
        }

       
    }
}
