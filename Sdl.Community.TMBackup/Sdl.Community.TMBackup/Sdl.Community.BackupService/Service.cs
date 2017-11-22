using Sdl.Community.BackupService.Models;

namespace Sdl.Community.BackupService
{
	public class Service
	{
		public JsonRequestModel GetJsonInformation()
		{
			Persistence persistence = new Persistence();
			JsonRequestModel result = persistence.ReadFormInformation();

			return result;
		}
	}
}