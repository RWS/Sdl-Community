using System;
using System.Globalization;
using Sdl.Community.XLIFF.Manager.Common;

namespace Sdl.Community.XLIFF.Manager.Model
{
	public class ProjectFileActionModel: BaseModel, IDisposable
	{	
		public ProjectFileActionModel()
		{			
			Action = Enumerators.Action.None;
		}

		public ProjectInfoModel ProjectInfo { get; set; }

		public Enumerators.Action Action { get; set; }		

		public string Id { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public DateTime Date { get; set; }		

		public CultureInfo TargetLanguage { get; set; }

		public string Message { get; set; }

		
		// Options
		

		// Segments etc...

		public void Dispose()
		{
		}			
	}
}
