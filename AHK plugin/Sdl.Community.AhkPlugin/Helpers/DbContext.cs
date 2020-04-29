using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using Sdl.Community.AhkPlugin.Model;

namespace Sdl.Community.AhkPlugin.Helpers
{
	public class DbContext
	{
		private readonly string  _defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
			"SDL Community", "AhkMasterScript", "Ahk.db");
		public Task<MasterScript> GetMasterScript()
		{

			using (var db = new LiteDatabase(_defaultPath))
			{
				var masterScriptCollection = db.GetCollection<MasterScript>("masterScript");
				var masterScript = masterScriptCollection.FindOne(m => m.Name.Contains("AhkMasterScript.ahk"));

				if (masterScript != null) return Task.FromResult(masterScript);
			}
			return null;
		}

		public Task UpdateScript(MasterScript masterScript)
		{
			using (var db = new LiteDatabase(_defaultPath))
			{
				var masterScriptCollection = db.GetCollection<MasterScript>("masterScript");
				var master = masterScriptCollection.FindOne(m => m.Name.Contains("AhkMasterScript.ahk"));
				if (master != null)
				{
					master.Location = masterScript.Location;
					master.Name = masterScript.Name;
					master.Scripts = masterScript.Scripts;
				}
				masterScriptCollection.Update(master);
			}
			return Task.FromResult(true);
		}

		public Task RemoveScripts(List<Script> scriptsToBeRemoved)
		{
			using (var db = new LiteDatabase(_defaultPath))
			{
				var masterScriptCollection = db.GetCollection<MasterScript>("masterScript");
				var master = masterScriptCollection.FindOne(m => m.Name.Contains("AhkMasterScript.ahk"));
				if (master != null)
				{
					foreach (var toBeRemoved in scriptsToBeRemoved)
					{
						var corespondingScript = master.Scripts.FirstOrDefault(s => s.ScriptId.Equals(toBeRemoved.ScriptId));
						if (corespondingScript != null)
						{
							master.Scripts.Remove(corespondingScript);
						}
					}
					masterScriptCollection.Update(master);
				}
			}
			return Task.FromResult(true);
		}
		public async Task<List<Script>> GetScriptsFromMaster()
		{
			var masterScript = await GetMasterScript();
			return masterScript.Scripts;
		}
	}
}
