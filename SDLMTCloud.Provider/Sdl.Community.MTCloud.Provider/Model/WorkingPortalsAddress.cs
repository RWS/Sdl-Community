using System;

namespace Sdl.Community.MTCloud.Provider.Model
{
	public enum WorkingPortal
	{
		UEPortal,
		USPortal
	}

	internal class WorkingPortalsAddress
	{
		internal static string GetWorkingPortalAddress(WorkingPortal viewModelSelectedWorkingPortal)
		{
			switch (viewModelSelectedWorkingPortal)
			{
				case WorkingPortal.UEPortal:
					return Constants.MTCloudTranslateAPIUriEU;
				case WorkingPortal.USPortal:
					return Constants.MTCloudTranslateAPIUriUS;
				default:
					throw new ArgumentOutOfRangeException(nameof(viewModelSelectedWorkingPortal), viewModelSelectedWorkingPortal, null);
			}
		}
	}
}
