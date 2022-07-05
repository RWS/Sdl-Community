using System;
using System.IO;

namespace PretranslateProjectsCreatedFromTemplateSample.Helpers
{
	public static class DirectoryHelper
	{
		public static DirectoryInfo GetDestinationDirectory(string newProjectPath)
		{
			var path = GeneratePath(newProjectPath);
			return Directory.CreateDirectory(path);
		}

		private static string GeneratePath(string newProjectPath)
		{
			while (true)
			{
				var path = $"{newProjectPath}_{GetGuid()}";
				//if (Directory.Exists(path)) continue;

				return path;
			}
		}

		private static string GetGuid()
		{
			return Guid.NewGuid().ToString();
		}
	}
}