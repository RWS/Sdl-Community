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

		//public string TestFile01PolyglotXliff12
		//{
		//	get { return Path.Combine(ProjectPath, GetSampleFilePath("Xliff12", "Polyglot", "SampleFile01.sdlxliff.xliff")); }
		//}

		//public string TestFile01SdlXliff12
		//{
		//	get { return Path.Combine(ProjectPath, GetSampleFilePath("Xliff12", "Sdl", "SampleFile01.sdlxliff.xliff")); }
		//}

		//public string TestFile02PolyglotXliff12
		//{
		//	get { return Path.Combine(ProjectPath, GetSampleFilePath("Xliff12", "Polyglot", "SampleFile02.sdlxliff.xliff")); }
		//}

		//public string TestFile02SdlXliff12
		//{
		//	get { return Path.Combine(ProjectPath, GetSampleFilePath("Xliff12", "Sdl", "SampleFile02.sdlxliff.xliff")); }
		//}

		public string GetSampleFilePath(string version, string support, string name)
		{
			return Path.Combine(ProjectPath, $"TestFiles\\{version}\\{support}\\{name}");
		}

		public string GetSampleFilePath(string support, string name)
		{
			return Path.Combine(ProjectPath, $"TestFiles\\{support}\\{name}");
		}
	}
}
