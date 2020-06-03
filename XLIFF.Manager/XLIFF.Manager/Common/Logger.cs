namespace Sdl.Community.XLIFF.Manager.Common
{
	public class Logger
	{
		//private static bool Initialized { get; set; }

		//public static void Setup(PathInfo pathInfo, string fileTarget)
		//{
		//	if (Initialized)
		//	{
		//		return;
		//	}

		//	Initialized = true;

		//	var config = new LoggingConfiguration();
		//	var assembly = Assembly.GetExecutingAssembly();

		//	var target = new FileTarget(fileTarget)
		//	{
		//		FileName = Path.Combine(pathInfo.ApplicationLogsFolderPath, "log." + GetDateToString() + ".txt"),

		//		// Roll over the log every 10 MB
		//		ArchiveAboveSize = 10000000,
		//		ArchiveNumbering = ArchiveNumberingMode.Date,			
		//		ArchiveFileName = pathInfo.ApplicationLogsFolderPath + "/" + assembly.GetName().Name + ".log.{#####}.txt"
		//	};
			
		//	config.AddTarget(target);						
		//	config.AddRuleForAllLevels(target, fileTarget);
			
		//	LogManager.Configuration = config;

		//	LogManager.GetCurrentClassLogger().Info("Started");
		//}

		//private static string GetDateToString()
		//{
		//	var now = DateTime.Now;
		//	var value = now.Year
		//	            + "" + now.Month.ToString().PadLeft(2, '0')
		//	            + "" + now.Day.ToString().PadLeft(2, '0')
		//	            + "" + now.Hour.ToString().PadLeft(2, '0')
		//	            + "" + now.Minute.ToString().PadLeft(2, '0')
		//	            + "" + now.Second.ToString().PadLeft(2, '0');

		//	return value;
		//}
	}
}
