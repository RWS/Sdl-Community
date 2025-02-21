using System.Collections.Generic;

namespace ExpressionEvaluator.Operators
{
    internal class OperatorCollection : Dictionary<string, IOperator>
    {
        private readonly List<char> _firstlookup = new List<char>();

        public new void Add(string key, IOperator op)
        {
            _firstlookup.Add(key[0]);
            base.Add(key, op);
        }

        public bool ContainsFirstKey(char key)
        {
            return _firstlookup.Contains(key);
        }

        public bool IsOperator(string c)
        {
            int i = 0;
            return IsOperator(c, ref i) != null;
        }

        // operators are of variable length, so we need to test at the current 
        // position for multiple lengths
        public string IsOperator(string str, ref int p)
        {
            string op = null;

            // Check id the first char in the string at the current position is 
            // part of an operator... (slight speedup? maybe not)

            if (ContainsFirstKey(str[p]))
            {
                string pop;
                if (str.Substring(p).Length > 1)
                {
                    pop = str.Substring(p, 2);
                    if (ContainsKey(pop))
                    {
                        p++;
                        op = pop;
                    }
                }
                if (op == null)
                {
                    pop = str.Substring(p, 1);
                    if (ContainsKey(pop))
                        op = pop;
                }
            }

            // return the operator we found, or null otherwise
            return op;
        }
    }
}