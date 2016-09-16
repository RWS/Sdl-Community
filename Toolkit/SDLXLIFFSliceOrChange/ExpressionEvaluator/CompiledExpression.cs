using System;
using System.Linq.Expressions;

namespace ExpressionEvaluator
{
    public class CompiledExpression<T> : ExpressionCompiler
    {
        private Func<T> _compiledMethod = null;

        public CompiledExpression()
        {
            Parser = new Parser();
            Parser.TypeRegistry = TypeRegistry;

        }

        public CompiledExpression(string expression)
        {
            Parser = new Parser(expression);
            Parser.TypeRegistry = TypeRegistry;
        }

        public Func<T> Compile()
        {
            if (Expression == null) Expression = BuildTree(); 
            return Expression.Lambda<Func<T>>(Expression).Compile();
        }

        protected override void ClearCompiledMethod()
        {
            _compiledMethod = null;
        }

        public T Eval()
        {
            if (_compiledMethod == null) _compiledMethod = Compile();
            return _compiledMethod();
        }
    }

    public class CompiledExpression : ExpressionCompiler
    {
        private Func<object> _compiledMethod = null;

        public CompiledExpression()
        {
            Parser = new Parser();
            Parser.TypeRegistry = TypeRegistry;
        }

        public CompiledExpression(string expression)
        {
            Parser = new Parser(expression);
            Parser.TypeRegistry = TypeRegistry;
        }

        public Func<object> Compile()
        {
            if (Expression == null) Expression = BuildTree();
            return Expression.Lambda<Func<object>>(Expression.Convert(Expression, typeof(object))).Compile();
        }

        protected override void ClearCompiledMethod()
        {
            _compiledMethod = null;
        }

        public object Eval()
        {
            if (_compiledMethod == null) _compiledMethod = Compile();
            return _compiledMethod();
        }

    }
}
