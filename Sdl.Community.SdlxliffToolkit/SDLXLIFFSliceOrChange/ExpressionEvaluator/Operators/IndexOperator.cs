using System;
using System.Linq.Expressions;

namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Operators
{
    internal class IndexOperator : Operator<Func<Expression, Expression, Expression>>
    {
        public IndexOperator(string value, int precedence, bool leftassoc, Func<Expression, Expression, Expression> func)
            : base(value, precedence, leftassoc, func)
        {
        }
    }
}