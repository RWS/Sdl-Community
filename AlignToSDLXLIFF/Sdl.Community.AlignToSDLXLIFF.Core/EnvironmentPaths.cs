using System;
using System.IO;

namespace Sdl.Community.Amgen.Core
{
	public class EnvironmentPaths
    {
		private string _myDocumentsPath;
		private string _myTmPath;
		private readonly string _myPathName;

		public EnvironmentPaths(string myPathName)
		{
			_myPathName = myPathName;
		}

		public string MyDocumentsPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_myDocumentsPath))
					return _myDocumentsPath;

				_myDocumentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _myPathName);
				if (!Directory.Exists(_myDocumentsPath))
					Directory.CreateDirectory(_myDocumentsPath);

				return _myDocumentsPath;
			}
		}

		public string MyTmPath
		{
			get
			{
				if (!string.IsNullOrEmpty(_myTmPath))
					return _myTmPath;

				_myTmPath = Path.Combine(MyDocumentsPath, "TM");
				if (!Directory.Exists(_myTmPath))
					Directory.CreateDirectory(_myTmPath);

				return _myTmPath;
			}
		}
	}
}