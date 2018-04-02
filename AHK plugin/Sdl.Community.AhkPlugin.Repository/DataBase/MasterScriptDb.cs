using System;
using System.Collections.Generic;
using System.IO;
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
			using (var session = RavenContext.Current.CreateSession())
			{
				var masterScript = session.Query<MasterScript>().FirstOrDefault();
				if (masterScript == null)
				{
					var master = new MasterScript
					{

						ScriptId = Guid.NewGuid().ToString(),
						Name = "AhkMasterScript.ahk",
						Location = GetDefaultPath(),
						Scripts = new List<Script>()
					};
					CreateMasterScript(master);
				}
				if (masterScript != null)
				{
					return Task.FromResult(masterScript);
				}
				var masterScriptNew = session.Query<MasterScript>().FirstOrDefault();
				return Task.FromResult(masterScriptNew);
			}
		}
		private string GetDefaultPath()
		{
			var defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"SDL Community", "AhkMasterScript");
			if (!Directory.Exists(defaultPath))
			{
				Directory.CreateDirectory(defaultPath);
			}
			return defaultPath;
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

		public Task RemoveScripts(List<Script> scriptsToBeRemoved)
		{
			using (var session = RavenContext.Current.CreateSession())
			{
				var script = GetMasterScript().Result;
				var masterScript = session.Query<MasterScript, MasterScriptById>()
					.FirstOrDefault(s => s.ScriptId.Equals(script.ScriptId));
				if (masterScript != null)
				{
					foreach (var toBeRemoved in scriptsToBeRemoved)
					{
						var corespondingScript = masterScript.Scripts.FirstOrDefault(s => s.ScriptId.Equals(toBeRemoved.ScriptId));
						if (corespondingScript != null)
						{
							masterScript.Scripts.Remove(corespondingScript);
						}
						
					}
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
