using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemory;
using TMX_Lib.Db;
using TMX_Lib.Search;

namespace TMXTests
{
	public class TestMongoDb
	{
		[Fact]
		public async Task TestSample4()
		{
			var root = "..\\..\\..\\..";
			var db = new TmxMongoDb("localhost:27017", "sample4");
			await db.ImportToDbAsync($"{root}\\SampleTestFiles\\#4.tmx");
			var search = new TmxSearch(db);
			await search.LoadLanguagesAsync();
			var segment = new Segment();
			segment.Add("This document contains both the Interserve Construction Health and Safety Code for Subcontractors and the Sustainability Code for Subcontractors.");
			var result = await search.Search(new SearchSettings(), segment, new LanguagePair("en-GB", "es-MX"));
			Assert.True(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "Este documento contiene el Interserve Construcción Código de Salud y Seguridad de los subcontratistas y la sostenibilidad Código de los subcontratistas.");

			segment = new Segment();
			segment.Add("En el cumplimiento de estas metas, lograr nuestra visión de ser el socio de confianza para todos aquellos con quienes tenemos una relación, accionistas, clientes, empleados, proveedores, miembros de la comunidad en la que estamos trabajando, o de cualquier otro grupo o persona.");
			result = await search.Search(new SearchSettings(), segment, new LanguagePair("es-MX", "en-GB"));
			Assert.True(result.Count == 1 && result[0].TranslationProposal.TargetSegment.ToPlain() == "In meeting these goals we will achieve our vision of being the trusted Partner to all those with whom we have a relationship be they shareholders, customers, employees, suppliers, members of the community in which we are working, or any other group or individual.");
		}
	}
}
