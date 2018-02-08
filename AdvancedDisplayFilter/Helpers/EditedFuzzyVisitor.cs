using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;

namespace Sdl.Community.Plugins.AdvancedDisplayFilter.Helpers
{
	public class EditedFuzzyVisitor
	{
		public bool IsEditedFuzzy(ISegment segment)
		{
			//_isEditedFuzzy = false;
			//for 100% edited
			if ((bool)segment.Properties?.TranslationOrigin?.OriginType.Equals("auto-propagated"))
			{
				return true;
			}

			if (segment.Properties?.TranslationOrigin?.OriginBeforeAdaptation?.OriginBeforeAdaptation != null)
			{
				//for 100% fuzzy which is not edited but is picked as edited
				if (segment.Properties.TranslationOrigin.TextContextMatchLevel.ToString().Equals("Source"))
				{
					return false;
				}

				if (segment.Properties.TranslationOrigin.OriginBeforeAdaptation.OriginBeforeAdaptation.OriginType != "source")
				{
					return true;
				}
				
			}
			//if (!ContainsFuzzy(segment))
			//{
			//	return false;
			//}
			return false;
		}

		private bool ContainsFuzzy(ISegment segment)
		{
			if ((bool) segment.Properties?.TranslationOrigin?.MatchPercent.Equals(0) ||
			    (bool) segment.Properties?.TranslationOrigin.Equals("mt"))
			{
				return false;
			}
			return true;
		}
	
	}
}
