using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.NumberVerifier.Model
{
	public class PartsList
	{
		
		public List<string> Separators { get; }

		private readonly List<(string, string)> _indexedList = new List<(string, string)>();

		public List<string> InitialPartsList
		{
			get { return _indexedList.Select(obj => obj.Item1).ToList(); }
		}

		public string this[string key]
		{
			get => _indexedList.FirstOrDefault(n => n.Item1 == key).Item2;
			set => _indexedList.Add((key,value));
		}

		public List<string> NormalizedPartsList => _indexedList.Select(n => n.Item2).ToList();

		public string this[int index]
		{
			get => _indexedList[index].Item2;
		}

		public PartsList(List<string> initialParts, List<string> normalizedParts, List<string> separators)
		{
			Separators = separators;
			if (initialParts is null) return;

			for (var i = 0; i < initialParts.Count; i++)
			{
				_indexedList.Add((initialParts[i], normalizedParts[i]));
			}
		}
	}
}