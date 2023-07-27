namespace InterpretBank.TermbaseViewer.Model
{
	public class TermModel
	{
		public long Id { get; set; }
		public string CommentAll { get; set; }
		public string Extra1 { get; set; }
		public string Extra2 { get; set; }
		public string Text { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as TermModel;
			return other != null && CommentAll == other.CommentAll && Extra1 == other.Extra1 && Extra2 == other.Extra2 && Text == other.Text;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (CommentAll != null ? CommentAll.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Extra1 != null ? Extra1.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Extra2 != null ? Extra2.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Text != null ? Text.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}