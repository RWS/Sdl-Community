using System;

namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Tokens
{
    internal class Token
    {
        public object Value;
        public bool IsIdent;
        public bool IsOperator;
        public bool IsType;
        public Type Type;
        public int ArgCount;
        public int Ptr;
        public bool IsCast;
    }
}