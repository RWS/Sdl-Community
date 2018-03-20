// Using Dictionary<> for the trie continuations is slightly faster during lookup, but
//  doesn't allow enumeration of the trie leaves

using System;
using System.Collections.Generic;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	/// <summary>
	/// An abstract iterator which can be used to iterate through a <see cref="Trie&lt;T, V&gt;"/>. The
	/// iterator can be thought of pointing to a node in the trie, or to <c>null</c> if 
	/// the iterator's current position is invalid.
	/// </summary>
	/// <typeparam name="T">The trie's element type (<see cref="Trie&lt;T, V&gt;"/>)</typeparam>
	/// <typeparam name="V">The trie's value type (<see cref="Trie&lt;T, V&gt;"/>)</typeparam>
	public abstract class TrieIterator<T, V>
		: IEnumerable<KeyValuePair<IList<T>, V>>
	{
		/// <summary>
		/// Traverse the trie from the current node, given the <paramref name="key"/>.
		/// </summary>
		/// <returns>true if the iteration was successful, false otherwise (in which case
		/// the iterator should no longer be used).</returns>
		public abstract bool Traverse(T key);
		/// <summary>
		/// Returns true if the iterator points to a valid location in the trie, and false
		/// otherwise (in which case the iterator should no longer be used).
		/// </summary>
		public abstract bool IsValid { get; }
		/// <summary>
		/// Returns the value associated with the current node. If the iterator
		/// does not point to a valid location, an exception may be thrown or the 
		/// default value for <typeparam name="V"/>
		/// may be returned.
		/// <remarks>It is recommended to call <see cref="IsValid"/> to check whether
		/// the current location is valid before this method is used.</remarks>
		/// </summary>
		public abstract V Value { get; }
		/// <summary>
		/// Returns true if the current node is a final node in the trie, which means that it
		/// has associated values. Note that final nodes are not identical to leaf nodes: 
		/// you can continue iteration even from a final node.
		/// </summary>
		public abstract bool IsFinal { get; }
		/// <summary>
		/// Returns the path traversed so far.
		/// </summary>
		public abstract IList<T> Path { get; }

		/// <summary>
		/// Returns an enumerator which can be used to enumerate all items "at or below" the 
		/// current node.
		/// </summary>
		/// <returns>An enumerator</returns>
		public abstract IEnumerator<KeyValuePair<IList<T>, V>> GetEnumerator();

		/// <summary>
		/// Returns an enumerator which can be used to enumerate all items "at or below" the 
		/// current node.
		/// </summary>
		/// <returns>An enumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Provides an enumerator which can be used to iterate through a trie's leaf nodes.
	/// </summary>
	/// <typeparam name="T">The label type</typeparam>
	/// <typeparam name="V">The leaf (value) type</typeparam>
	public abstract class TrieEnumerator<T, V> : IEnumerator<KeyValuePair<IList<T>, V>>
	{
		/// <summary>
		/// See <see cref="IEnumerator.Current"/>
		/// </summary>
		public abstract KeyValuePair<IList<T>, V> Current
		{
			get;
		}

		/// <summary>
		/// See <see cref="IDisposable.Dispose"/>
		/// </summary>
		public abstract void Dispose();

		/// <summary>
		/// See <see cref="IEnumerator.Current()"/>
		/// </summary>
		object System.Collections.IEnumerator.Current
		{
			get { return Current; }
		}

		/// <summary>
		/// See <see cref="IEnumerator.MoveNext()"/>
		/// </summary>
		public abstract bool MoveNext();

		/// <summary>
		/// See <see cref="IEnumerator.Reset()"/>
		/// </summary>
		public abstract void Reset();
	}

	/// <summary>
	/// A trie structure which contains sequences of T (an element type) which define paths to nodes of type V (the value type). For example,
	/// if <typeparamref name="T"/> is <c>char</c>, and <typeparamref name="V"/> is <c>int</c>, you can use a trie to map sequences of 
	/// characters (strings) to integer numbers.
	/// <para>Instances of this class are not thread-safe for parallel insertions, but are safe to use
	/// from multiple threads if only lookups are performed.</para>
	/// </summary>
	/// <remarks>Tries are relatively slow to build (slower than e.g. HashSet&lt;string&gt;) but are fast 
	/// during lookup.</remarks>
	/// <typeparam name="T">The element type. Paths are expressed as sequences of T.</typeparam>
	/// <typeparam name="V">The node value type. Used to store information at final positions.</typeparam>
	public class Trie<T, V> : IEnumerable<KeyValuePair<IList<T>, V>>
		where T: IComparable<T>
	{

		private class Node : IComparable<Node>
		{
			private List<Node> _Children;
			private V _Value;
			private T _Label;
			private bool _IsFinal;

			public Node(Node parent, T edgeLabel)
			{
				_Children = null;
				_Value = default(V);
				_IsFinal = false;
				_Label = edgeLabel;
			}

			public bool IsFinal
			{
				get { return _IsFinal; }
			}

			public V Value
			{
				get { return _Value; }
			}

			public T Label
			{
				get { return _Label; }
			}

			public void SetValue(V v)
			{
				_Value = v;
				_IsFinal = true;
			}

			public Node Traverse(T c, bool create)
			{
				Node result = null;

				int idx = ~0;
				if (_Children != null)
				{
					Node dummy = new Node(null, c);
					idx = _Children.BinarySearch(dummy);
					if (idx >= 0)
						return _Children[idx];
				}

				if (create)
				{
					result = new Node(this, c);

					if (_Children == null)
					{
						_Children = new List<Node>();
						_Children.Add(result);
					}
					else
					{
						System.Diagnostics.Debug.Assert(idx < 0);
						_Children.Insert(~idx, result);
					}
				}

				return result;
			}

			public T GetChildLabelAt(int index)
			{
				return _Children[index]._Label;
			}

			public Node GetChildNodeAt(int index)
			{
				return _Children[index];
			}

			public int ChildCount
			{
				get 
				{
					return _Children == null ? 0 : _Children.Count;
				}
			}

			public int CompareTo(Node other)
			{
				return _Label.CompareTo(other._Label);
			}
		}

		private class TrieIteratorImpl : TrieIterator<T, V>
		{
			private Node _Node;
			private List<T> _Path;

			public TrieIteratorImpl(Node n)
			{
				if (n == null)
					throw new ArgumentNullException();
				_Node = n;
				_Path = new List<T>();
			}

			public override bool Traverse(T key)
			{
				if (_Node == null)
					throw new InvalidOperationException();
				_Node = _Node.Traverse(key, false);
				if (_Node != null)
				{
					_Path.Add(_Node.Label);
				}
				return _Node != null;
			}

			public override bool IsValid
			{
				get { return _Node != null; }
			}

			public override V Value
			{
				get
				{
					if (_Node == null)
						throw new InvalidOperationException();
					return _Node.Value;
				}
			}

			public override IList<T> Path
			{
				get { return _Path; }
			}

			public override bool IsFinal
			{
				get
				{
					if (_Node == null)
						throw new InvalidOperationException();
					return _Node.IsFinal;
				}
			}

			public override IEnumerator<KeyValuePair<IList<T>, V>> GetEnumerator()
			{
				if (_Node == null)
					throw new InvalidOperationException();
				return new TrieEnumeratorImpl(_Node, _Path);
			}

		}

		private class TrieEnumeratorImpl : TrieEnumerator<T, V>
		{
			private Node _Root;
			private List<Position> _Path;
			private Nullable<KeyValuePair<IList<T>, V>> _Current;
			private IList<T> _Prefix;

			private struct Position
			{
				public Position(Node n)
				{
					Node = n;
					NextEdge = -1;
				}

				public Node Node;
				public int NextEdge;
			}

			public TrieEnumeratorImpl(Node root, IList<T> prefix)
			{
				_Root = root;
				_Path = new List<Position>();
				_Prefix = prefix;
				_Current = null;
				Reset();
			}

			public TrieEnumeratorImpl(Node root)
				: this(root, null)
			{
			}

			public override KeyValuePair<IList<T>, V> Current
			{
				get
				{
					if (_Current == null || !_Current.HasValue)
						throw new InvalidOperationException();
					return _Current.Value;
				}
			}

			private void SetCurrent()
			{
				if (_Path == null || _Path.Count == 0)
				{
					_Current = null;
				}
				else
				{
					Position tos = Peek();
					Node n = tos.Node;

					if (n == null || !n.IsFinal)
						_Current = null;
					else
					{
						List<T> path = new List<T>();

						if (_Prefix != null)
							path.AddRange(_Prefix);

						for (int p = 1; p < _Path.Count; ++p)
							path.Add(_Path[p].Node.Label);

						_Current = new KeyValuePair<IList<T>, V>(path,
							n.Value);
					}
				}
			}

			public override void Dispose()
			{
				_Path.Clear();
			}

			public override bool MoveNext()
			{
				_Current = null;

				while (_Path.Count > 0)
				{
					Position pos = Pop();

					if (pos.NextEdge < 0)
					{
						pos.NextEdge++;

						// "at" node - prefix iteration
						if (pos.Node.IsFinal)
						{
							Push(pos);
							SetCurrent();
							return true;
						}
					}

					System.Diagnostics.Debug.Assert(pos.NextEdge >= 0);

					if (pos.NextEdge < pos.Node.ChildCount)
					{
						// "traverse" edge pos.NextEdge and inc

						T label = pos.Node.GetChildLabelAt(pos.NextEdge);
						Node n = pos.Node.GetChildNodeAt(pos.NextEdge);

						pos.NextEdge++;
						Push(pos);

						Push(new Position(n));
					}

					// otherwise: just pop
				}

				return false;
			}

			private void Push(Position p)
			{
				_Path.Add(p);
			}

			private Position Peek()
			{
				return _Path[_Path.Count - 1];
			}

			private Position Pop()
			{
				Position result = _Path[_Path.Count - 1];
				_Path.RemoveAt(_Path.Count - 1);
				return result;
			}

			public override void Reset()
			{
				_Path.Clear();
				Push(new Position(_Root));
				_Current = null;
			}
		}

		private Node _Root;

		/// <summary>
		/// Constructs a new Trie. 
		/// </summary>
		public Trie()
		{
			_Root = new Node(null, default(T));
		}

		/// <summary>
		/// Empties the trie and removes all nodes.
		/// </summary>
		public void Clear()
		{
			_Root = new Node(null, default(T));
		}

		/// <summary>
		/// Returns a <see cref="TrieIterator&lt;T, V&gt;"/> which can be used to traverse the trie. The
		/// iterator will "point" to the trie's root node initially.
		/// </summary>
		public TrieIterator<T, V> GetIterator()
		{
			if (_Root == null)
				throw new InvalidOperationException();
			return new TrieIteratorImpl(_Root);
		}

		/// <summary>
		/// Adds or extends the node at path <paramref name="s"/>. If the path already points to 
		/// a final node, its value will be overwritten. 
		/// </summary>
		/// <param name="s">The key sequence</param>
		/// <param name="nodeValue">A value to add or set on the final node for <paramref name="s"/>.s</param>
		public void Add(IList<T> s, V nodeValue)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			Node n = Traverse(s, true);
			System.Diagnostics.Debug.Assert(n != null);
			n.SetValue(nodeValue);
		}

		/// <summary>
		/// Updates the value at the final node for the key sequence <paramref name="s"/>
		/// to <paramref name="nodeValue"/>. If the key sequence is not contained
		/// in the trie, an exception is thrown.
		/// </summary>
		/// <param name="s">The key sequence</param>
		/// <param name="nodeValue">The new node value to be set for the final node for <paramref name="s"/>.</param>
		public void Update(IList<T> s, V nodeValue)
		{
			if (s == null)
				throw new ArgumentNullException("s");
			Node n = Traverse(s, true);
			if (n == null)
				throw new InvalidOperationException();
			n.SetValue(nodeValue);
		}

		private Node Traverse(IList<T> s)
		{
			return Traverse(s, false);
		}

		private Node Traverse(IList<T> s, bool create)
		{
			Node n = _Root;
			if (s != null)
			{
				for (int i = 0; i < s.Count && n != null; ++i)
					n = n.Traverse(s[i], create);
			}
			return n;
		}

		/// <summary>
		/// Returns the node value at path <paramref name="s"/> of the trie, or default(<typeparamref name="V"/>) if the
		/// trie does not contain the key sequence.
		/// </summary>
		public V Lookup(IList<T> s)
		{
			Node n = Traverse(s);
			if (n != null && n.IsFinal)
				return n.Value;
			else
				return default(V);
		}

		/// <summary>
		/// Returns true if a node at path <paramref name="s"/> exists and has values. 
		/// </summary>
		/// <param name="s">The key sequence to look up</param>
		/// <returns>true if the trie contains the key sequence <paramref name="s"/>
		/// and has values for that key sequence.</returns>
		/// <remarks>Note that the method also returns <c>false</c> if the key
		/// sequence is contained in the trie, but does not have any associated
		/// values.</remarks>
		public bool Contains(IList<T> s)
		{
			Node n = Traverse(s);
			return n != null && n.IsFinal;
		}

		/// <summary>
		/// Returns true if a node at path <paramref name="s"/> exists and has values. 
		/// </summary>
		/// <param name="s">The key sequence to look up</param>
		/// <param name="firstValue">The value for <paramref name="s"/>, 
		/// if <paramref name=">s"/> is contained in the trie, or default(<typeparamref name="V"/>) otherwise.</param>
		/// <returns>true if the trie contains the key sequence <paramref name="s"/>
		/// and has a value for that key sequence.</returns>
		public bool Contains(IList<T> s, out V value)
		{
			Node n = Traverse(s);
			value = default(V);
			if (n != null && n.IsFinal)
			{
				value = n.Value;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Returns <c>true</c> if enumeration is supported by this instance, and <c>false</c> otherwise.
		/// If enumeration is not supported, calling <see cref="GetEnumerator"/> (or using a <c>foreach</c> 
		/// statement) will raise an exception.
		/// </summary>
		public bool IsEnumerationSupported
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Obtains an enumerator which can be used to enumerate the trie's leaf nodes, providing
		/// access to the path to the leaf and the value at that leaf.
		/// <para>Before obtaining an enumerator, you should first call <see cref="IsEnumerationSupported"/>. If that
		/// returns <c>false</c>, enumeration is unsupported and calling  <see cref="GetEnumerator"/> will
		/// raise an exception.</para>
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<KeyValuePair<IList<T>, V>> GetEnumerator()
		{

			if (_Root == null)
				throw new InvalidOperationException();

			return new TrieEnumeratorImpl(_Root);
		}

		/// <summary>
		/// Obtains an enumerator which can be used to enumerate the trie's leaf nodes, providing
		/// access to the path to the leaf and the value list at the leaf.
		/// </summary>
		/// <returns>An enumerator</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

}
