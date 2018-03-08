using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.FST
{
    internal class SequenceNode : Node
    {
        private List<Node> _Sequence;

        public SequenceNode()
        {
            _Sequence = new List<Node>();
        }

		public override string GetExpression()
		{
			System.Text.StringBuilder builder = new StringBuilder("(");
			for (int n = 0; n < _Sequence.Count; ++n)
				builder.Append(_Sequence[n].GetExpression());
			builder.Append(")");
			return builder.ToString();
		}

		public void Add(Node n)
        {
            n.Owner = this;
            _Sequence.Add(n);
        }

        public override FST GetFST()
        {
            if (_Sequence.Count == 0)
                throw new Exception("Unexpected");

            Node n = _Sequence[0];
            FST result = n.GetFST();
            for (int i = 1; i < _Sequence.Count; ++i)
            {
                FST sub = _Sequence[i].GetFST();
                result.Concatenate(sub);
            }
            return result;
        }
    }
}
