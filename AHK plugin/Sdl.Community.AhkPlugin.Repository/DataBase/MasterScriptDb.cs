using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Indexes;
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
		public Task CreateMasterScript(MasterScript script)
		{
			using (var session = RavenContext.Current.CreateSession())
			{
				session.Store(script);
				session.SaveChanges();
			}
			return Task.FromResult(true);
		}

		public Task UpdateScript(MasterScript script)
		{
			using (var session = RavenContext.Current.CreateSession())
			{
				var masterScript = session.Query<MasterScript, MasterScriptById>()
					.FirstOrDefault(s => s.ScriptId.Equals(script.ScriptId));
				if (masterScript != null)
				{
					masterScript.Location = script.Location;
					masterScript.Scripts = script.Scripts;
					session.Store(masterScript);
					session.SaveChanges();
				}
			}
			return Task.FromResult(true);
		}

	}

	public class MasterScriptById : AbstractIndexCreationTask<MasterScript>
	{
		public MasterScriptById()
		{
			Map = scripts => from script in scripts
				select new { script.ScriptId };
		}
	}
}
