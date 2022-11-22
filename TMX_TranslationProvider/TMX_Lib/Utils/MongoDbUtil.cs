using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TMX_Lib.Db;
using TMX_Lib.TmxFormat;
using TmxTranslationUnit = TMX_Lib.TmxFormat.TmxTranslationUnit;

namespace TMX_Lib.Utils
{
	public static class MongoDbUtil
	{

		private static (IReadOnlyList<Db.TmxText> texts, Db.TmxTranslationUnit tu) TmxTranslationUnitToMongo(TmxTranslationUnit tu, ulong id)
		{
			Db.TmxTranslationUnit dbTU = new Db.TmxTranslationUnit
			{
				ID = id, 
				CreationAuthor = tu.CreationAuthor,
				CreationDate = tu.CreationTime,
				ChangeAuthor = tu.ChangeAuthor,
				ChangeDate = tu.ChangeTime,
				XmlProperties = tu.XmlProperties,
				TuAttributes = tu.TuAttributes,
			};

			// FIXME handle multiple languages
			var dbSource = new Db.TmxText
			{
				TranslationUnitID = id,
				Language = tu.SourceLanguage,
				Text = tu.SourceText,
				FormattedText = tu.SourceFormattedText,
			};
			var dbTarget = new Db.TmxText
			{
				TranslationUnitID = id,
				Language = tu.TargetLanguage,
				Text = tu.TargetText,
				FormattedText = tu.TargetFormattedText,
			};

			return (new[] { dbSource, dbTarget }, dbTU);
		}

		public static async Task ImportToDbAsync(TmxParser parser, TmxMongoDb db)
		{
			Stopwatch watch;
			try
			{
				await db.ClearAsync();
				watch = Stopwatch.StartNew(); 
				await db.AddMetasAsync(new []
				{
					new TmxMeta { Type = "Header", Value = parser.Header.Xml,},
					new TmxMeta { Type = "Source Language", Value = parser.Header.SourceLanguage,},
					new TmxMeta { Type = "Target Language", Value = parser.Header.TargetLanguage,},
					new TmxMeta { Type = "Domains", Value = string.Join(", ", parser.Header.Domains),},
					new TmxMeta { Type = "Creation Date", Value = parser.Header.CreationDate?.ToLongDateString() ?? "unknown",},
					new TmxMeta { Type = "Author", Value = parser.Header.Author,},
				});

				ulong id = 1;
				while (true)
				{
					var TUs = parser.TryReadNextTUs();
					if (TUs == null)
						break;

					var dbTexts = new List<TmxText>();
					var dbTUs = new List<Db.TmxTranslationUnit>();
					foreach (var tu in TUs)
					{
						var dbTU = TmxTranslationUnitToMongo(tu, id++);
						dbTexts.AddRange(dbTU.texts);
						dbTUs.Add(dbTU.tu);
					}

					await db.AddTranslationUnitsAsync(dbTUs);
					await db.AddTextsAsync(dbTexts);
				}

				// languages are known only after everything has been imported
				var languages = parser.Languages().Select(l => new TmxLanguage { Language = l }).ToList();
				await db.AddLanguagesAsync(languages);
			}
			catch (Exception e)
			{
				throw new TmxException("Import to db failed", e);
			}
			Debug.WriteLine($"import complete, took {watch.ElapsedMilliseconds} ms");
		}
	}
}
