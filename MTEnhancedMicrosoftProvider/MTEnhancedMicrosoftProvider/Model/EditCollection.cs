using System.Collections.Generic;

namespace MTEnhancedMicrosoftProvider.Model
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