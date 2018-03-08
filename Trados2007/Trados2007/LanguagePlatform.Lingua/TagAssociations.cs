using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sdl.LanguagePlatform.Lingua
{
	public class TagAssociations : IEnumerable<TagAssociation>
	{
		private List<TagAssociation> _Associations;
		private Dictionary<int, int> _SrcPositionIdx;
		private Dictionary<int, int> _TrgPositionIdx;

		public TagAssociations()
		{
			_Associations = new List<TagAssociation>();
			_SrcPositionIdx = new Dictionary<int, int>();
			_TrgPositionIdx = new Dictionary<int, int>();
		}

		public int Count
		{
			get { return _Associations.Count; }
		}

		public TagAssociation this[int p]
		{
			get { return _Associations[p]; }
		}

		public void Add(PairedTag srcTag, PairedTag trgTag)
		{
			Add(srcTag, trgTag, Core.EditDistance.EditOperation.Undefined);
		}

		public void Add(PairedTag srcTag, PairedTag trgTag,
			Core.EditDistance.EditOperation op)
		{
			System.Diagnostics.Debug.Assert(srcTag != null || trgTag != null);

			if (op == Core.EditDistance.EditOperation.Undefined)
			{
				if (srcTag == null)
					op = Core.EditDistance.EditOperation.Insert;
				else if (trgTag == null)
					op = Core.EditDistance.EditOperation.Delete;
				else
					op = Core.EditDistance.EditOperation.Change;
			}

			int idx = _Associations.Count;
			_Associations.Add(new TagAssociation(srcTag, trgTag, op));

			if (srcTag != null)
			{
				System.Diagnostics.Debug.Assert(srcTag.Start < srcTag.End);
				_SrcPositionIdx.Add(srcTag.Start, idx);
				_SrcPositionIdx.Add(srcTag.End, idx);
			}

			if (trgTag != null)
			{
				System.Diagnostics.Debug.Assert(trgTag.Start < trgTag.End);
				_TrgPositionIdx.Add(trgTag.Start, idx);
				_TrgPositionIdx.Add(trgTag.End, idx);
			}
		}

		/// <summary>
		/// true iff the two positions are start or end position of an associated paired tag, 
		/// i.e. the source position is the source tag's start or end position, and the target
		/// position is the target tag's start or end position
		/// </summary>
		public bool AreAssociated(int sourcePosition, int targetPosition)
		{
			TagAssociation ta = GetBySourcePosition(sourcePosition);
			if (ta == null)
				return false;
			System.Diagnostics.Debug.Assert(ta.SourceTag != null
				&& (ta.SourceTag.Start == sourcePosition || ta.SourceTag.End == sourcePosition));

			if (ta.TargetTag == null)
				return false;

			if (ta.TargetTag.Start == targetPosition || ta.TargetTag.End == targetPosition)
				return true;

			return false;
		}

		/// <summary>
		/// Returns the tag association which is related to the specified source position, 
		/// which may either be the start or end position of a tag.
		/// </summary>
		public TagAssociation GetBySourcePosition(int p)
		{
			int idx;
			if (!_SrcPositionIdx.TryGetValue(p, out idx))
				return null;
			return _Associations[idx];
		}

		/// <summary>
		/// Returns the tag association operation which is related to the specified source position, 
		/// which may either be the start or end position of a tag.
		/// </summary>
		public Core.EditDistance.EditOperation GetOperationBySourcePosition(int p)
		{
			int idx;
			if (!_SrcPositionIdx.TryGetValue(p, out idx))
				return Core.EditDistance.EditOperation.Undefined;
			return _Associations[idx].Operation;
		}

		/// <summary>
		/// Returns the tag association which is related to the specified target position, 
		/// which may either be the start or end position of a tag.
		/// </summary>
		public TagAssociation GetByTargetPosition(int p)
		{
			int idx;
			if (!_TrgPositionIdx.TryGetValue(p, out idx))
				return null;
			return _Associations[idx];
		}

		/// <summary>
		/// Returns the tag association operation which is related to the specified target position, 
		/// which may either be the start or end position of a tag.
		/// </summary>
		public Core.EditDistance.EditOperation GetOperationByTargetPosition(int p)
		{
			int idx;
			if (!_TrgPositionIdx.TryGetValue(p, out idx))
				return Core.EditDistance.EditOperation.Undefined;
			return _Associations[idx].Operation;
		}

		public IEnumerator<TagAssociation> GetEnumerator()
		{
			return _Associations.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _Associations.GetEnumerator();
		}
	}
}
