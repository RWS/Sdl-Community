using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua.FST
{
    /// <summary>
    /// An internal helper class which represents an RX expression tree
    /// which later will be converted into an FST
    /// </summary>
    internal abstract class Node
    {
        private Node _Owner;

        public Node()
        {
            _Owner = null;
        }

        public Node Owner
        {
            get { return _Owner; }
            set { _Owner = value; }
        }

        public abstract FST GetFST();

		public abstract string GetExpression();

		/// <summary>
		/// <see cref="object.ToString()"/>
		/// </summary>
		/// <returns>A string representation of the object, for display purposes.</returns>
		public override string ToString()
		{
			return GetExpression();
		}
    }
}
