using System;
using System.Threading.Tasks;
using TMX_Lib.Db;
using TMX_Lib.TmxFormat;

namespace TMX_Lib.Utils
{
	public static class Util
	{
		public static async Task ImportToDbAsync(TmxParser parser, TmxMongoDb db)
		{
			try
			{
				await db.ClearAsync();

			}
			catch (Exception e)
			{
				throw new TmxException("Import to db failed", e);
			}
		}
	}
}
