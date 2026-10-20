using System.Linq;

namespace Sdl.Community.DsiViewer.Model
{
	public class DSITagModel
	{
		public string Id { get; set; }

		public string StartTag { get; set; }

		public string EndTag { get; set; }

		public string Content { get; set; }

		public string Attributes { get; set; }

		/// <summary>Tag name without markup, e.g. "cf" for "&lt;cf&gt;" or a placeholder's "&lt;x\&gt;".</summary>
		public string Name => StartTag?.Trim('<', '>', '\\', '/', ' ');

		public bool HasAttributes => !string.IsNullOrWhiteSpace(Attributes);

		public bool HasContent => !string.IsNullOrWhiteSpace(Content);

		/// <summary>One-line description for screen readers and tooltips, e.g. "cf 2, bold=True, cardiac surgeon".</summary>
		public string Summary => string.Join(", ", new[] { $"{Name} {Id}", Attributes?.Trim(), Content?.Trim() }.Where(part => !string.IsNullOrWhiteSpace(part)));
	}
}
