namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Tokens
{
    internal class OpToken : Token
    {
        public OpToken()
        {
            IsOperator = true;
            ArgCount = 0;
        }
    }
}