using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class DatabaseContextService : DbContext
	{
		public DbSet<SearchCache> SearchCaches { get; set; }
		public DbSet<SearchResultModel> CacheSearchResults { get; set; }

		public DatabaseContextService(string projectName) : base(projectName)
		{
			var cacheDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community",
				"IATEProviderCache");
			Directory.CreateDirectory(cacheDirectoryPath);

			var dbFilePath = Path.Combine(cacheDirectoryPath, $"{projectName}.mdf");
			Database.Connection.ConnectionString = $@"Data Source=(localdb)\mssqllocaldb;Integrated Security=True;MultipleActiveResultSets=True;AttachDbFilename={dbFilePath}";
			Database.CreateIfNotExists();
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Database does not pluralize table names
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}
