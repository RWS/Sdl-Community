using System;
using System.Collections.Generic;
using Sdl.Community.MTCloud.Provider.Model;

namespace Sdl.Community.MTCloud.Provider.Interfaces
{
	public interface ISegmentSupervisor
	{
		string Improvement { get; }
		string OriginalTargetText { get; }

		event EventHandler ImprovementAvailable;
		Dictionary<string, ImprovedTarget> Improvements { get; set; }
	}
}