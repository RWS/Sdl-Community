namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Tokens
{
    internal class MemberToken : OpToken
    {
        public string Name { get; set; }

        public MemberToken()
        {
            Value = ".";
        }
    }
}