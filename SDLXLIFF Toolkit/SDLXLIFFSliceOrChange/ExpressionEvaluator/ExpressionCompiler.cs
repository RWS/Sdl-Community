using System.Linq.Expressions;

namespace ExpressionEvaluator
{
    public abstract class ExpressionCompiler
    {
        protected Expression Expression = null;
        protected Parser Parser = null;
        protected TypeRegistry TypeRegistry = new TypeRegistry();
        protected string Pstr = null;

        public string StringToParse
        {
            get { return Parser.StringToParse; }
            set { 
                Parser.StringToParse = value;
                Expression = null;
                ClearCompiledMethod();
            }
        }

        public void RegisterDefaultTypes()
        {
            TypeRegistry.RegisterDefaultTypes();
        }

        public void RegisterType(string key, object type)
        {
            TypeRegistry.Add(key, type);
        }

        public Expression BuildTree()
        {
            return Parser.BuildTree();
        }

        protected abstract void ClearCompiledMethod();

        public void Parse()
        {
            Parser.Parse();
        }

    }
}