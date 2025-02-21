using Sdl.Community.Utilities.TMTool.Lib.TMHelpers;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using System;
using System.IO;

namespace Sdl.Community.Utilities.TMTool.Lib
{
	public class TMFileManager
	{
		public delegate void OnProgressDelegate(double progress, int operationType);
		public event OnProgressDelegate OnProgress;

		#region fields
		private FileBasedTranslationMemory _tm;
		private string _password;
		private double _progress;
		#endregion

		#region properties
		/// <summary>
		/// is TM password protected
		/// </summary>
		public bool IsProtected
		{
			get
			{
				return _tm.IsProtected;
			}
		}
		/// <summary>
		/// source TM file path
		/// </summary>
		public string TMFilePath
		{
			get;
			private set;
		}
		/// <summary>
		/// number of TUs in source TM file
		/// </summary>
		public int TUsCount
		{ get; private set; }
		/// <summary>
		/// number of TUs exported to TMX
		/// </summary>
		public int TUsExportedCount
		{ get; private set; }
		/// <summary>
		/// number of TUs imported to new TM
		/// </summary>
		public int TUsImportedCount
		{ get; private set; }
		#endregion

		#region constructors
		/// <summary>
		/// create new TMFileManager
		/// </summary>
		/// <param name="filePath">source file path to process</param>
		public TMFileManager(string filePath)
		{
			TMFilePath = filePath;

			ValidateTMFilePath();

			_tm = new FileBasedTranslationMemory(TMFilePath);
			TUsCount = _tm.GetTranslationUnitCount();
		}
		/// <summary>
		/// create new TMFileManager
		/// </summary>
		/// <param name="filePath">source file path to process</param>
		/// <param name="psw">admin password file is protected with</param>
		public TMFileManager(string filePath, string psw)
		{
			TMFilePath = filePath;
			_password = psw;

			ValidateTMFilePath();

			_tm = new FileBasedTranslationMemory(TMFilePath, _password);
			TUsCount = _tm.GetTranslationUnitCount();
		}
		#endregion

		/// <summary>
		/// opens protected TM with admin password
		/// </summary>
		/// <param name="psw">TM admin password</param>
		/// <returns>true if succeeded</returns>
		public bool OpenWithPassword(string psw)
		{
			_password = psw;
			try
			{
				_tm = new FileBasedTranslationMemory(TMFilePath, _password);
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// performs index revert operation
		/// </summary>
		/// <param name="newFilePath">path for file with reverted index</param>
		/// <param name="settings">import operation settings</param>
		public void ReverseIndex(string newFilePath, DataImportSettings settings)
		{
			// export TM to TMX
			Exporter tmExp = new Exporter();
			tmExp.OnProgress += new Exporter.OnProgressDelegate(updateProgress);
			tmExp.Export(_tm);
			TUsExportedCount = tmExp.TUsExported;

			// create new TM with reverted index
			TMCreator tmCreator = new TMCreator(_tm.FilePath);
			tmCreator.OnProgress += new TMCreator.OnProgressDelegate(updateProgress);
			tmCreator.CreateNewTM(_tm, true);

			if (_password != null && _password.Length > 0 && settings.PreservePsw)
				tmCreator.SetAdminProtection(_password);

			// if any TUs were exported - import data to new TM
			if (tmExp.TMXPath.Length > 0 && File.Exists(tmExp.TMXPath))
			{
				if (tmCreator.ImportData(settings, tmExp.TMXPath, TUsExportedCount))
				{
					FileBasedTranslationMemory tmImport = new FileBasedTranslationMemory(tmCreator.FilePath);
					TUsImportedCount = tmImport.GetTranslationUnitCount();
				}
				else throw new InvalidDataException();

				File.Delete(tmExp.TMXPath);
			}

			// manage files
			newFilePath = FileHelper.ChangeFileName(newFilePath, @"{0}\{1}({2}).sdltm");
			ManageFileNames(tmCreator.FilePath, newFilePath, false);
		}

		/// <summary>
		/// removes duplicates from file
		/// </summary>
		/// <param name="settings">import operation settings</param>
		public void RemoveDuplicates(DataImportSettings settings)
		{
			settings.OverwriteExistingTUs = true;

			// export TM to TMX
			Exporter tmExp = new Exporter();
			tmExp.OnProgress += new Exporter.OnProgressDelegate(updateProgress);
			tmExp.Export(_tm);
			TUsExportedCount = tmExp.TUsExported;

			// create new TM with reverted index
			TMCreator tmCreator = new TMCreator(_tm.FilePath);
			tmCreator.OnProgress += new TMCreator.OnProgressDelegate(updateProgress);
			tmCreator.CreateNewTM(_tm, false);

			if (_password != null && _password.Length > 0 && settings.PreservePsw)
				tmCreator.SetAdminProtection(_password);

			// if any TUs were exported - import data to new TM
			if (tmExp.TMXPath.Length > 0 && File.Exists(tmExp.TMXPath))
			{
				if (tmCreator.ImportData(settings, tmExp.TMXPath, TUsExportedCount))
				{
					FileBasedTranslationMemory tmImport = new FileBasedTranslationMemory(tmCreator.FilePath);
					TUsImportedCount = tmImport.GetTranslationUnitCount();
				}
				else throw new InvalidDataException(Properties.StringResource.errImportTMX);

				File.Delete(tmExp.TMXPath);
			}

			// manage files
			ManageFileNames(tmCreator.FilePath, TMFilePath, settings.CreateBackupFile);
		}

		#region private
		private void ValidateTMFilePath()
		{
			if (!FileHelper.FileExists(TMFilePath))
				throw new FileNotFoundException(string.Format(Properties.StringResource.errFileNotFound, TMFilePath));
		}

		private void ManageFileNames(string createdFilePath, string requestedFilePath, bool isCreateBackup)
		{
			if (isCreateBackup)
				if (!FileHelper.CopyFile(requestedFilePath, string.Format("{0}.backup", requestedFilePath), true))
					throw new InvalidDataException(Properties.StringResource.errBackupFile);

			if (FileHelper.CopyFile(createdFilePath, requestedFilePath, true))
				File.Delete(createdFilePath);
			else throw new InvalidDataException(string.Format(Properties.StringResource.errCreateFile,
				Path.GetFileName(requestedFilePath),
				Path.GetDirectoryName(requestedFilePath)));
		}

		private void updateProgress(double progress, int operationType)
		{
			switch (operationType)
			{
				case 0:
					_progress = Math.Round(progress * 0.2);
					break;
				case 1:
					_progress = Math.Round(20 + progress * 0.8);
					break;
			}

			if (this.OnProgress != null)
			{
				this.OnProgress(_progress, operationType);
			}
		}
		#endregion
	}
}
