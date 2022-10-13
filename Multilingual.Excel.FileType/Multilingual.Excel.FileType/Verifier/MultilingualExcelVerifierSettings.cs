using System;

namespace Multilingual.Excel.FileType.Verifier
{
	/// <summary>
	/// Used to identify when the subtitling settings have changed
	/// 
	/// TODO: expose event in IntegrationAPI to identify when project settings have changed.
	/// </summary>
	public class MultilingualExcelVerifierSettings
	{
		static MultilingualExcelVerifierSettings() { }

		private static readonly object LockObject = new object();
		private static MultilingualExcelVerifierSettings _instance;

		public static MultilingualExcelVerifierSettings Instance
		{
			get
			{
				lock (LockObject)
				{
					return _instance ?? (_instance = new MultilingualExcelVerifierSettings());
				}
			}
		}

		public event EventHandler SavedSettings;
		
		internal void OnSavedSettings()
		{
			SavedSettings?.Invoke(this, EventArgs.Empty);
		}
	}
}
