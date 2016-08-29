using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Operators
{
    internal class MethodOperator : Operator<Func<Expression, string, List<Expression>, Expression>>
    {
        public MethodOperator(string value, int precedence, bool leftassoc,
                              Func<Expression, string, List<Expression>, Expression> func)
            : base(value, precedence, leftassoc, func)
        {
        }
    }
}