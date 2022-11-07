using System.Collections.Generic;

namespace GoogleTranslatorProvider.Models
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