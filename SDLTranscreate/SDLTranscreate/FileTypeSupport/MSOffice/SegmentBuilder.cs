using System;
using System.Collections.Generic;
using Sdl.Community.Transcreate.FileTypeSupport.MSOffice.Model;
using Sdl.Community.Transcreate.FileTypeSupport.SDLXLIFF;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.BilingualApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.Formatting;
using Sdl.FileTypeSupport.Framework.NativeApi;

namespace Sdl.Community.Transcreate.FileTypeSupport.MSOffice
{
	public class SegmentBuilder
	{
		public SegmentBuilder()
		{
			ItemFactory = DefaultDocumentItemFactory.CreateInstance();
			PropertiesFactory = DefaultPropertiesFactory.CreateInstance();
			FormattingFactory = PropertiesFactory.FormattingItemFactory;
		}

		public IDocumentItemFactory ItemFactory { get; }

		public IPropertiesFactory PropertiesFactory { get; }

		public IFormattingItemFactory FormattingFactory { get; }


	}
}
