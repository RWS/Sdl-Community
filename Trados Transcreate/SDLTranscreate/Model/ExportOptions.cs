using System.Collections.Generic;
using System.Linq;
using Trados.Transcreate.Common;

namespace Trados.Transcreate.Model
{
	public class ExportOptions : BaseModel
	{
		private List<string> _excludeFilterIds;

		public ExportOptions()
		{
			XliffSupport = Enumerators.XLIFFSupport.xliff12sdl;
			IncludeTranslations = true;
			CopySourceToTarget = true;
			IncludeBackTranslations = false;
			ExcludeFilterIds = new List<string>();
			//ExcludeFilterIds.Add("Locked");
		}

		public Enumerators.XLIFFSupport XliffSupport { get; set; }

	    public bool IncludeTranslations { get; set; }

	    public bool CopySourceToTarget { get; set; }

		public bool IncludeBackTranslations { get; set; }

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
