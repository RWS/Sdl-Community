using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Multilingual.Excel.FileType.FileType.Processors
{
	internal class DocumentPostTweaker: AbstractFilePostTweaker
	{
		protected override void Tweak(INativeOutputFileProperties properties)
		{
			var originalOutputFilePath = properties.OutputFilePath;


			
		}
	}
}
