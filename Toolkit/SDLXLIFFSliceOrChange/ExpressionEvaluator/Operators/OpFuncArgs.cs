using System.Collections.Generic;
using System.Linq.Expressions;
using ExpressionEvaluator.Tokens;

namespace ExpressionEvaluator.Operators
{
    internal class OpFuncArgs
    {
        public Queue<Token> TempQueue;
        public Stack<Expression> ExprStack;
        //public Stack<String> literalStack;
        public Token T;
        public IOperator Op;
        public List<Expression> Args;
    }
}