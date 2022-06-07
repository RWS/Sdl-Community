using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Utilities.SplitSDLXLIFF.Lib.Common
{
	// Used when assigning the text from the WordCount() method in TextHelper.cs 
	public class Text : IText, ITextProperties
	{
		private string _text = string.Empty; 
		public Text(string text)
		{
			_text = text;
		}
		public object Clone()
		{
			return null;
		}

		public int UniqueId { get; set; }
		public void AcceptVisitor(IMarkupDataVisitor visitor)
		{
		}

		public void RemoveFromParent()
		{
		}

		public IAbstractMarkupDataContainer Parent { get; set; }
		public int IndexInParent { get; }
		public IParagraph ParentParagraph { get; }

		string ITextProperties.Text
		{
			get => _text;
			set => value = _text;
		}

		public ITextProperties Properties { get; set; }

		public IText Split(int fromIndex)
		{
			return null;
		}
		
	}

	// Implement ITextProperties interface (used to assign the Properties to _iTxt.Properties when calling VisitText() method in TextHelper.cs
	public class TextProperties : ITextProperties
	{
		private string _text = string.Empty;
		public TextProperties(string text)
		{
			_text = text;
		}
		public object Clone()
		{
			return null;
		}

		public string Text
		{
			get => _text;
			set => value = _text;
		}
	}
}