using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.IO;
using NLog;
using Sdl.Community.IATETerminologyProvider.Model;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class DatabaseContextService : DbContext
	{
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public DbSet<SearchCache> SearchCaches { get; set; }

		public DatabaseContextService(string projectName) : base(projectName)
		{
			try
			{
				var cacheDirectoryPath = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community",
					"IATEProviderCache");
				Directory.CreateDirectory(cacheDirectoryPath);

				var dbFilePath = Path.Combine(cacheDirectoryPath, $"{projectName}.mdf");
				Database.Connection.ConnectionString =
					$@"Data Source=(localdb)\mssqllocaldb;Integrated Security=True;MultipleActiveResultSets=True;AttachDbFilename={dbFilePath}";

				_logger.Info($"--> Trying to create db with name: {projectName} at following path: {dbFilePath}");
				var dbCreated = Database.CreateIfNotExists();
				_logger.Info($"--> {dbFilePath} was created: {dbCreated} (if false the dbalready exists at that path)");
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			// Database does not pluralize table names
			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
		}
	}
}
