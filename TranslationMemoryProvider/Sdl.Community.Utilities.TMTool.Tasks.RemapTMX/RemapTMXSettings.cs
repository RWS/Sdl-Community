using System;
using System.IO;
using Sdl.Community.Utilities.TMTool.Task;

namespace Sdl.Community.Utilities.TMTool.Tasks.RemapTMX
{
	public class RemapTMXSettings : ISettings
	{
		#region Fields

		/// <summary>
		/// Default dll's path.
		/// </summary>
		private string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the RemapTMXSettings class.
		/// </summary>
		public RemapTMXSettings()
		{
			this.ResetToDefaults();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets output tmx files folder.
		/// </summary>
		public string TargetFolder { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to save flavoured Trados 2007 TMX into target TM folder.
		/// </summary>
		/// <value>
		/// 	<c>true</c> to save; otherwise, <c>false</c>.
		/// </value>
		public bool SaveIntoTargetFolder { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// Validates settings.
		/// </summary>
		/// <param name="errMsg">Error message.</param>
		/// <returns>True if settings valid, otherwise - false.</returns>
		public bool ValidateSettings(out string errMsg)
		{
			if (!Directory.Exists(this.TargetFolder))
			{
				errMsg = Properties.Resources.OutputFolderError;
				return false;
			}

			errMsg = string.Empty;
			return true;
		}

		/// <summary>
		/// Resets settigns to defaults.
		/// </summary>
		public void ResetToDefaults()
		{
			this.TargetFolder = string.Format(this.defaultPath);
			this.SaveIntoTargetFolder = false;
		}
		#endregion
	}
}