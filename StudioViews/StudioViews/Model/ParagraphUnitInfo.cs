using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.StudioViews.Model
{
	public class ParagraphUnitInfo
	{
		public IParagraphUnit ParagraphUnit { get; set; }

		public IFileProperties FileProperties { get; set; }
	}
}
