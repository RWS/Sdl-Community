using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Sdl.Community.AhkPlugin.Repository.Raven
{
	public  class RavenContext:IDisposable
	{
		private readonly EmbeddableDocumentStore _store;
		private static readonly Lazy<RavenContext> Context = new Lazy<RavenContext>(() => new RavenContext());
		private IDocumentSession _session;

		private RavenContext()
		{
			_store = new EmbeddableDocumentStore
			{
				DataDirectory = "Data",
				DefaultDatabase = "AhKDb"
			};
			_store.Configuration.Settings["Raven/WorkingDir"] = GetWorkingDirectory();
			_store.Configuration.Initialize();
			_store.Initialize();
			IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), Store);
		}
		public  IDocumentStore Store
		{
			get
			{
				if (_store == null)
					throw new InvalidOperationException(
						"IDocumentStore has not been initialized.");
				return _store;
			}
		}
		public IDocumentSession CurrentSession => _session ?? (_session = _store.OpenSession());
		public  IDocumentSession CreateSession()
		{
			return _store.OpenSession();
		}

		public void CloseCurrentSession()
		{
			if (_session == null) return;
			_session.Dispose();
			_session = null;
		}
		public static RavenContext Current => Context.Value;

		private  static string GetWorkingDirectory()
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				@"SDL Community\Ahk");
		}

		public void Dispose()
		{
			CloseCurrentSession();
			_store.Dispose();
		}
	}
}
