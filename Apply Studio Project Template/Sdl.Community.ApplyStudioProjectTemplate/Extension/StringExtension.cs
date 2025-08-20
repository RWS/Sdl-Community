namespace Sdl.Community.ApplyStudioProjectTemplate.Extension
{
    public static class StringExtension
    {
        public static string ToEnumString(this string text)
        {
            if (text.Split(' ').Length == 2)
                text = text.Replace(" ", "");
            return text;
        }

        public static string ToUiString(this string text)
        {
            if (text.Contains("pend"))
                text = text.Insert(5, " ");
            return text;
        }
    }
}