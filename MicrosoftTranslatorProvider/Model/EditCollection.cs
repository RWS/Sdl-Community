using System.Collections.Generic;

namespace MicrosoftTranslatorProvider.Model
{
	public class EditCollection
	{
		public List<EditItem> Items { get; set; }

		public EditCollection()
		{
			Items = new List<EditItem>();
		}
	}
}