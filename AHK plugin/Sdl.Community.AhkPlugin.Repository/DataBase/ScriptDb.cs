using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client.Indexes;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.Raven;

namespace Sdl.Community.AhkPlugin.Repository.DataBase
{
	public class ScriptDb
	{
		private bool _disposed;

		public  Task<List<Script>> GetAllScripts()
		{
			//ThrowIfDisposed();
			using (var session = RavenContext.Current.CreateSession())
			{
				var allScripts =  session.Query<Script>().ToList();
				return Task.FromResult(allScripts);
			}
		}

		public Task AddNewScript(Script script)
		{
			//ThrowIfDisposed();
			using (var session = RavenContext.Current.CreateSession())
			{
				session.Store(script);
				session.SaveChanges();
			}
			return Task.FromResult(true);
		}

		public Task RemoveScripts(List<Script> scripts)
		{
			//ThrowIfDisposed();
			using (var session = RavenContext.Current.CreateSession())
			{
				foreach (var script in scripts)
				{
					var result = session.Query<Script, ScriptById>().FirstOrDefault(s => s.ScriptId.Equals(script.ScriptId));
					session.Delete(result);
				}
				session.SaveChanges();
			}
			return Task.FromResult(true);
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(GetType().Name);
		}
		public class ScriptById: AbstractIndexCreationTask<Script>
		{
			public ScriptById()
			{
				Map = scripts => from script in scripts
					select new { script.ScriptId };
			}
		}
	}
}
