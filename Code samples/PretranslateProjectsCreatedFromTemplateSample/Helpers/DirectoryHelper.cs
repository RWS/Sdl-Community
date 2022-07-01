using System;
using System.IO;

namespace PretranslateProjectsCreatedFromTemplateSample.Helpers
{
	public static class DirectoryHelper
	{
		/// <summary>
		/// Method that returns a unique folder at the path provided
		/// </summary>
		/// <param name="newProjectPath"></param>
		/// <returns></returns>
		public static DirectoryInfo GetDestinationDirectory(string newProjectPath)
		{
			return Directory.CreateDirectory($"{newProjectPath}_{GetDateTimeToString(DateTime.Now)}");
		}

		private static string GetDateTimeToString(DateTime dateTime)
		{
			var value = dateTime.Year +
						dateTime.Month.ToString().PadLeft(2, '0') +
						dateTime.Day.ToString().PadLeft(2, '0') +
						"-" +
						dateTime.Hour.ToString().PadLeft(2, '0') +
						dateTime.Minute.ToString().PadLeft(2, '0') +
						dateTime.Second.ToString().PadLeft(2, '0');
			return value;
		}
	}
}