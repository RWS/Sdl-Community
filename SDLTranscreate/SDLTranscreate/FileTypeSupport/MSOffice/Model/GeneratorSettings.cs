using System.Drawing;
using System.Xml.Serialization;

namespace Trados.Transcreate.FileTypeSupport.MSOffice.Model
{
	public class GeneratorSettings
	{
		//public enum ExclusionType { Category, Status }

		//public enum UpdateSegmentMode { All, TrackedOnly }

		private decimal _columnWidth;

		public GeneratorSettings()
		{
			ResetToDefaults();
		}

		public void ResetToDefaults()
		{
			//FileNamePrefix = "";
			ColumnWidth = 75;
			ExtractComments = true;
			ContextMatchColor = Color.LightGray;
			ExactMatchColor = Color.PaleGreen;
			FuzzyMatchColor = Color.Wheat;
			NoMatchColor = Color.White;
			//IsContextMatchLocked = false;
			//IsExactMatchLocked = false;
			//IsFuzzyMatchLocked = false;
			//IsNoMatchLocked = false;
			//DontExportContext = false;
			//DontExportExact = false;
			//DontExportFuzzy = false;
			//DontExportNoMatch = false;

			//UpdateSegmentStatusNoTracked = false;
			//UpdateSegmentStatusTracked = false;
			//NewSegmentStatusAll = ConfirmationLevel.Draft;
			//NewSegmentStatusTrackedChanges = ConfirmationLevel.Draft;

			//ExcludeExportType = ExclusionType.Category;
			//ExcludedStatuses = new List<ConfirmationLevel>();

			//ImportUpdateSegmentMode = UpdateSegmentMode.All;
			//WarningWhenOverwrite = false;
			//BackupImport = false;
			ExportBackTranslations = false;
			ImportBackTranslations = false;
		}

		public bool ExportBackTranslations { get; set; }

		public bool ImportBackTranslations { get; set; }

		///// <summary>
		///// Prefix which will be used for generated file names.
		///// </summary>
		//public string FileNamePrefix
		//{
		//	get;
		//	set;
		//}

		/// <summary>
		/// Specify if comments from the bilingual file should be extracted
		/// </summary>
		public bool ExtractComments
		{
			get;
			set;
		}


		/// <summary>
		/// Value used for setting columns width in the Excel document. 
		/// As any value smaller than 75 makes no sense (too thin columns),
		/// by default all smaller values are changed to 75.
		/// </summary>
		public decimal ColumnWidth
		{
			get
			{
				if (_columnWidth < 10)
				{
					_columnWidth = 75;
				}
				return _columnWidth;
			}
			set { _columnWidth = value; }
		}

		///// <summary>
		///// Not used yet
		///// </summary>
		//public string LockPassword
		//{
		//	get;
		//	set;
		//}

		///// <summary>
		///// Specify if the no matches will be locked
		///// </summary>
		//public bool IsNoMatchLocked
		//{
		//	get;
		//	set;
		//}

		///// <summary>
		///// Specify if the fuzzy matches will be locked
		///// </summary>
		//public bool IsFuzzyMatchLocked
		//{
		//	get;
		//	set;
		//}

		///// <summary>
		///// Specify if the exact matches will be locked
		///// </summary>
		//public bool IsExactMatchLocked
		//{
		//	get;
		//	set;
		//}

		///// <summary>
		///// Specify if the context/perfect matches will be locked
		///// </summary>
		//public bool IsContextMatchLocked
		//{
		//	get;
		//	set;
		//}

		/// <summary>
		/// Color used to represent no match
		/// </summary>
		[XmlIgnore]
		public Color NoMatchColor
		{
			get
			{
				return Color.FromArgb(NoMatchColorSetting);
			}
			set
			{
				NoMatchColorSetting = value.ToArgb();
			}
		}

		public int NoMatchColorSetting
		{
			get;
			set;
		}

		/// <summary>
		/// Color used to represent fuzzy match
		/// </summary>
		[XmlIgnore]
		public Color FuzzyMatchColor
		{
			get
			{
				return Color.FromArgb(FuzzyMatchColorSetting);
			}
			set
			{
				FuzzyMatchColorSetting = value.ToArgb();
			}
		}

		public int FuzzyMatchColorSetting
		{
			get;
			set;
		}

		/// <summary>
		/// Color used to represent exact match
		/// </summary>
		[XmlIgnore]
		public Color ExactMatchColor
		{
			get
			{
				return Color.FromArgb(ExactMatchColorSetting);
			}
			set
			{
				ExactMatchColorSetting = value.ToArgb();
			}
		}

		public int ExactMatchColorSetting
		{
			get;
			set;
		}

		/// <summary>
		/// Color used to represent context/perfect match
		/// </summary>
		[XmlIgnore]
		public Color ContextMatchColor
		{
			get
			{
				return Color.FromArgb(ContextMatchColorSetting);
			}
			set
			{
				ContextMatchColorSetting = value.ToArgb();
			}
		}

		public int ContextMatchColorSetting
		{
			get;
			set;
		}

		//public ConfirmationLevel NewSegmentStatusAll
		//{
		//	get;
		//	set;
		//}

		//public ConfirmationLevel NewSegmentStatusTrackedChanges
		//{
		//	get;
		//	set;
		//}

		//public bool DontExportContext
		//{
		//	get;
		//	set;
		//}

		//public bool DontExportExact
		//{
		//	get;
		//	set;
		//}

		//public bool DontExportFuzzy
		//{
		//	get;
		//	set;
		//}

		//public bool DontExportNoMatch
		//{
		//	get;
		//	set;
		//}

		//public ExclusionType ExcludeExportType
		//{
		//	get;
		//	set;
		//}

		//public List<ConfirmationLevel> ExcludedStatuses
		//{
		//	get;
		//	set;
		//}

		//public UpdateSegmentMode ImportUpdateSegmentMode
		//{
		//	get;
		//	set;
		//}

		//public bool WarningWhenOverwrite
		//{
		//	get;
		//	set;
		//}

		//public bool BackupImport
		//{
		//	get;
		//	set;
		//}

		//public bool UpdateSegmentStatusTracked
		//{
		//	get;
		//	set;
		//}

		//public bool UpdateSegmentStatusNoTracked
		//{
		//	get;
		//	set;
		//}
	}
}
