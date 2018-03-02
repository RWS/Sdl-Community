using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.Raven;

namespace Sdl.Community.AhkPlugin.Repository.DataBase
{
	public class MasterScriptDb
	{
		public  Task<MasterScript> GetMasterScript()
		{
			using (var session =  RavenContext.Current.CreateSession())
			{
				var masterScript = session.Query<MasterScript>().FirstOrDefault();
				return Task.FromResult(masterScript);
			}
		}
	}
}
