using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.ViewModel;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class ReturnPackageWindowViewModelUnitTests
	{
		private static ReturnPackageWindowViewModel CreateViewModel()
		{
			var returnPackage = new ReturnPackage { ReturnFilesDetails = new List<ReturnFileDetails>() };
			return new ReturnPackageWindowViewModel(returnPackage, null, null, null);
		}

		[Fact]
		public void Encodings_OffersOnlyEncodingsUsableForZipEntryNames()
		{
			var encodings = CreateViewModel().Encodings.Select(e => Encoding.GetEncoding(e.CodePage));

			//zip entry names are byte strings in a code page (ANSI, OEM, DBCS) or UTF-8; ZipArchive rejects UTF-16
			//and UTF-32/UTF-7 would produce names no zip reader interprets
			Assert.All(encodings, encoding => Assert.False(encoding is UnicodeEncoding || encoding is UTF32Encoding || encoding is UTF7Encoding,
				$"{encoding.EncodingName} cannot be used for zip entry names"));
			//the code pages Transit installations actually use must survive the filter: Windows ANSI, DOS, a DBCS page, UTF-8
			Assert.All(new[] { 1252, 850, 932, 65001 }, codePage => Assert.Contains(encodings, encoding => encoding.CodePage == codePage));
		}

		[Fact]
		public void SelectedFileNameEncoding_DefaultsToTheSystemAnsiCodePage()
		{
			var viewModel = CreateViewModel();

			//Transit reads and writes zip entry names in the Windows ANSI code page
			Assert.Equal(Encoding.Default.CodePage, viewModel.SelectedFileNameEncoding.CodePage);
		}
	}
}
