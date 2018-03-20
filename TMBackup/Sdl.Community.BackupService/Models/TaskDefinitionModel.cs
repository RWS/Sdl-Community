﻿using System;

namespace Sdl.Community.BackupService.Models
{
	public class TaskDefinitionModel
	{
		public string TaskName { get; set; }
		public string TaskRunType { get; set; }
		public string Status { get; set; }
		public DateTime LastRun { get; set; }
		public DateTime NextRun { get; set; }
		public string Interval { get; set; }
	}
}