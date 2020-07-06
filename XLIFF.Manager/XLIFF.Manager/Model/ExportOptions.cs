using System.Collections.Generic;
using System.Linq;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ExportOptions : BaseModel
	{
		private List<string> _excludeFilterIds;

		public ExportOptions()
		{
			XliffSupport = Enumerators.XLIFFSupport.xliff12polyglot;
			IncludeTranslations = false;
			CopySourceToTarget = false;
			ExcludeFilterIds = new List<string>();
		}

		public Enumerators.XLIFFSupport XliffSupport { get; set; }

	    public bool IncludeTranslations { get; set; }

	    public bool CopySourceToTarget { get; set; }

		public List<string> ExcludeFilterIds
		{
			get => _excludeFilterIds;
			set
			{
				_excludeFilterIds = value?.Distinct().ToList();
				OnPropertyChanged(nameof(ExcludeFilterIds));
			}
		}
	}
}
