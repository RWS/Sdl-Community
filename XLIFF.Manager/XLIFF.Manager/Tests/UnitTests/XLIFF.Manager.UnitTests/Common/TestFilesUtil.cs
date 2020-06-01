using System;
using System.IO;

namespace XLIFF.Manager.UnitTests.Common
{
	public class TestFilesUtil
	{
		public string ProjectPath
		{
			get
			{
				var directoryInfo = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory);
				if (directoryInfo?.Parent != null)
				{
					return directoryInfo.Parent.FullName;
				}

				return string.Empty;
			}
		}

		public string SolutionPath
		{
			get
			{
				var directoryInfo = Directory.GetParent(ProjectPath);
				if (directoryInfo != null)
				{
					return directoryInfo.FullName;
				}

				return string.Empty;
			}
		}

		public string TestFilePolyglotXliff12
		{
			get { return Path.Combine(ProjectPath, GetTestFilePath("Xliff12", "Polyglot")); }
		}

		public string TestFileSdlXliff12
		{
			get { return Path.Combine(ProjectPath, GetTestFilePath("Xliff12", "Sdl")); }
		}

		private string GetTestFilePath(string version, string support)
		{
			return Path.Combine(ProjectPath, $"TestFiles\\{version}\\{support}\\File.sdlxliff.xliff");
		}
	}
}
