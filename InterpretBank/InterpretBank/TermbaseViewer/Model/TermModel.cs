namespace InterpretBank.TermbaseViewer.Model
{
	public class TermModel
	{
		public long Id { get; set; }
		public string CommentAll { get; set; }
		public string TargetTermComment1 { get; set; }
		public string TargetTermComment2 { get; set; }
		public string TargetTerm { get; set; }
		public string SourceTerm { get; set; }
		public string SourceTermComment1 { get; set; }
		public string SourceTermComment2 { get; set; }

		public override bool Equals(object obj)
		{
			return obj is TermModel other && Id == other.Id && CommentAll == other.CommentAll && TargetTermComment1 == other.TargetTermComment1 && TargetTermComment2 == other.TargetTermComment2 && TargetTerm == other.TargetTerm && SourceTerm == other.SourceTerm && SourceTermComment1 == other.SourceTermComment1 && SourceTermComment2 == other.SourceTermComment2;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Id.GetHashCode();
				hashCode = (hashCode * 397) ^ (CommentAll != null ? CommentAll.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTermComment1 != null ? TargetTermComment1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTermComment2 != null ? TargetTermComment2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (TargetTerm != null ? TargetTerm.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTerm != null ? SourceTerm.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTermComment1 != null ? SourceTermComment1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (SourceTermComment2 != null ? SourceTermComment2.GetHashCode() : 0);
				return hashCode;
			}
		}

		//public override bool Equals(object obj)
		//{
		//	var other = obj as TermModel;
		//	return other != null && CommentAll == other.CommentAll && Extra1 == other.Extra1 && Extra2 == other.Extra2 && TargetTerm == other.TargetTerm;
		//}

		//public override int GetHashCode()
		//{
		//	unchecked
		//	{
		//		var hashCode = (CommentAll != null ? CommentAll.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (Extra1 != null ? Extra1.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (Extra2 != null ? Extra2.GetHashCode() : 0);
		//		hashCode = (hashCode * 397) ^ (TargetTerm != null ? TargetTerm.GetHashCode() : 0);
		//		return hashCode;
		//	}
		//}
	}
}