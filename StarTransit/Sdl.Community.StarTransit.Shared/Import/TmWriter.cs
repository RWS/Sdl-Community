using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TmWriter: AbstractBilingualFileTypeComponent, IBilingualWriter, INativeOutputSettingsAware
	{
		private IPersistentFileConversionProperties _originalFileProperties;
		private INativeOutputFileProperties _nativeFileProperties;
		//private XmlDocument _targetFile;
		private TransitTextExtractor _textExtractor;

		public void Initialize(IDocumentProperties documentInfo)
		{
			throw new NotImplementedException();
		}

		public void Complete()
		{
			throw new NotImplementedException();
		}

		public void SetFileProperties(IFileProperties fileInfo)
		{
			throw new NotImplementedException();
		}

		public void FileComplete()
		{
			throw new NotImplementedException();
		}

		public void ProcessParagraphUnit(IParagraphUnit paragraphUnit)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void SetOutputProperties(INativeOutputFileProperties properties)
		{
			throw new NotImplementedException();
		}

		public void GetProposedOutputFileInfo(IPersistentFileConversionProperties fileProperties, IOutputFileInfo proposedFileInfo)
		{
			throw new NotImplementedException();
		}
	}
}
