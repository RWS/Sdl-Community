using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.ProjectAutomation.Core;
using Xunit;

namespace Sdl.Community.StarTransit.UnitTests
{
	public class ReturnPackageServiceUnitTests
	{
		// Transit ignores the zip UTF-8 flag and reads entry names as raw single-byte text in the selected code page
		// (a Transit-generated PPF stores "Störung" as 0xF6 with the flag clear), so the check is on the raw bytes.
		[Theory]
		[InlineData(1252)]
		[InlineData(850)]
		public void ExportFiles_FileNameWithUmlaut_EntryNameIsSingleByteInSelectedCodePage(int codePage)
		{
			const string transitFileName = "TEST_Störung.ENG";
			var workingFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
			Directory.CreateDirectory(workingFolder);
			try
			{
				File.WriteAllText(Path.Combine(workingFolder, transitFileName), "target");
				var prjFile = Path.Combine(workingFolder, "Test.PRJ");
				File.WriteAllLines(prjFile, new[] { "[Exchange]", "IsExchange=0" });
				var package = new ReturnPackage
				{
					FolderLocation = workingFolder,
					PathToPrjFile = prjFile,
					SelectedTargetFilesForImport = new List<ProjectFile>
					{
						CreateProjectFile(Path.Combine(workingFolder, transitFileName + ".sdlxliff"))
					}
				};

				new ReturnPackageService(null).ExportFiles(package, codePage);

				var (flags, nameBytes) = ReadCentralDirectoryEntry(Path.Combine(workingFolder, "Test.tpf"), ".ENG");
				Assert.Equal(0, flags & 0x800);
				Assert.Equal(Encoding.GetEncoding(codePage).GetBytes(transitFileName), nameBytes);
			}
			finally
			{
				Directory.Delete(workingFolder, true);
			}
		}

		// ProjectFile has an internal constructor; only LocalFilePath is read when the archive is built
		private static ProjectFile CreateProjectFile(string localFilePath)
		{
			var projectFile = (ProjectFile)FormatterServices.GetUninitializedObject(typeof(ProjectFile));
			typeof(ProjectFile).GetProperty(nameof(ProjectFile.LocalFilePath))?.SetValue(projectFile, localFilePath);
			return projectFile;
		}

		// Walks the zip central directory and returns the general-purpose flags and the raw name bytes
		// of the first entry whose name ends with the given ASCII suffix
		private static (int flags, byte[] nameBytes) ReadCentralDirectoryEntry(string zipPath, string asciiSuffix)
		{
			var bytes = File.ReadAllBytes(zipPath);
			var endOfCentralDirectory = bytes.Length - 22;
			while (BitConverter.ToUInt32(bytes, endOfCentralDirectory) != 0x06054b50) endOfCentralDirectory--;
			var position = (int)BitConverter.ToUInt32(bytes, endOfCentralDirectory + 16);

			while (BitConverter.ToUInt32(bytes, position) == 0x02014b50)
			{
				var flags = BitConverter.ToUInt16(bytes, position + 8);
				var nameLength = BitConverter.ToUInt16(bytes, position + 28);
				var extraLength = BitConverter.ToUInt16(bytes, position + 30);
				var commentLength = BitConverter.ToUInt16(bytes, position + 32);
				var nameBytes = new byte[nameLength];
				Array.Copy(bytes, position + 46, nameBytes, 0, nameLength);
				if (Encoding.ASCII.GetString(nameBytes).EndsWith(asciiSuffix)) return (flags, nameBytes);
				position += 46 + nameLength + extraLength + commentLength;
			}

			throw new InvalidDataException($"No entry ending with {asciiSuffix} in {zipPath}");
		}
	}
}
