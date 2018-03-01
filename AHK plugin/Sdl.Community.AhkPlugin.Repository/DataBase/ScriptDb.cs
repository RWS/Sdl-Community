using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Community.AhkPlugin.Model;
using Sdl.Community.AhkPlugin.Repository.Raven;

namespace Sdl.Community.AhkPlugin.Repository.DataBase
{
	public class ScriptDb
	{
		private bool _disposed;

		public  Task<List<Script>> GetAllScripts()
		{
			ThrowIfDisposed();
			using (var session = RavenContext.Current.CreateSession())
			{
				var allScripts =  session.Query<Script>().ToList();
				return Task.FromResult(allScripts);
			}
		}

		private void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(this.GetType().Name);
		}
	}
}
