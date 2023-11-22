using Newtonsoft.Json;
using Sdl.Core.Bcm.BcmConverters;
using Sdl.Core.Bcm.BcmConverters.FromBilingualApi;
using Sdl.Core.Bcm.BcmModel;
using Sdl.FileTypeSupport.Bilingual.SdlXliff;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Integration;
using File = System.IO.File;

namespace BCMToBilingualConverter
{
	public class Converter
	{
		private readonly PocoFilterManager _fileTypeManager;

		public Converter()
		{
			_fileTypeManager = new PocoFilterManager(true);
			//_fileTypeManager.AddFileTypeDefinition(new SdlXliffFilterComponentBuilder());
		}
		public void BilingualToBCM(string inputFilePath, string outputFilePath)
		{
			var converter = _fileTypeManager.GetConverterToBilingual(inputFilePath, null, null, null);
			var bcmExtractor = new BcmExtractor
			{
				BcmExtractionSettings = new BcmExtractionSettings
				{
					GenerateContextsDependencyFile = true,
					ProcessComments = true
				}
			};

			converter.AddBilingualProcessor(new BilingualContentHandlerAdapter(bcmExtractor));
			converter.Parse();
			
			var json = JsonConvert.SerializeObject(bcmExtractor.OutputDocument);

			File.WriteAllText(outputFilePath, json);
		}

		public void BCMToBilingual(string inputFilePath, string outputFilePath)
		{
			var json = File.ReadAllText(inputFilePath);
			var document = JsonConvert.DeserializeObject<Document>(json);
			var parser = new BcmParser(document);
			var converter = _fileTypeManager.GetConverterToDefaultBilingual(parser, outputFilePath);
			
			converter.Parse();
		}
	}
}
