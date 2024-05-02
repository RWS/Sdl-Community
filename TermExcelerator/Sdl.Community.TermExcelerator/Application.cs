using System;
using Sdl.Community.TermExcelerator.Services;

namespace Sdl.Community.TermExcelerator
{
	public static class ApplicationContext
	{
		private static PersistenceService _persistenceService;

		public static event Action SettingsChangedFromTellMeAction;

		public static PersistenceService PersistenceService => _persistenceService ??= new PersistenceService();

		public static void RaiseSettingsChangedFromTellMeAction()
		{
			SettingsChangedFromTellMeAction?.Invoke();
		}
	}
}