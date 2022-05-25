using System;
using Sdl.ProjectAutomation.Core;

namespace Sdl.Community.InSource
{
	public class ProjectRequest
	{
		public string Name { get; set; }

		public string[] Files { get; set; }

		public Guid NotificationId { get; set; }

		public string Path { get; set; }

		public ProjectTemplateInfo ProjectTemplate { get; set; }

		public override string ToString()
		{
			return Name;
		}
	}
}