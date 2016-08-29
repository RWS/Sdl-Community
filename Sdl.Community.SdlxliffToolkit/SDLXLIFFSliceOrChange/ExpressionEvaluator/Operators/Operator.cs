namespace Sdl.Community.SDLXLIFFSliceOrChange.ExpressionEvaluator.Operators
{
    internal abstract class Operator<T> : IOperator
    {
        public T Func;
        public string Value { get; set; }
        public int Precedence { get; set; }
        public int Arguments { get; set; }
        public bool LeftAssoc { get; set; }

        protected Operator(string value, int precedence, bool leftassoc, T func)
        {
            this.Value = value;
            this.Precedence = precedence;
            this.LeftAssoc = leftassoc;
            this.Func = func;
        }

    }
}
