using System;

namespace Sdl.Community.InvoiceAndQuotes.OpenXML
{
    public class Token
    {
        public String Text { get; set; }
        public object Value { get; set; }

        public Token()
        {
        }

        public Token(String text, object value)
        {
            Text = text;
            Value = value;
        }
    }
}
