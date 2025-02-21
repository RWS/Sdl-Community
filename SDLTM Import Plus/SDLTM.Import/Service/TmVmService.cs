using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using SDLTM.Import.Helpers;
using SDLTM.Import.Interface;
using SDLTM.Import.Model;

namespace SDLTM.Import.Service
{
    public class TmVmService:ITmVmService
    {
	    public void RemoveTms(IList tmsToRemove, ObservableCollection<TmDetails> tmsList)
	    {
		    var selectedItems = tmsToRemove.Cast<TmDetails>().ToList();
			foreach (var selectedTm in selectedItems)
			{
				var tmToRemove = tmsList.FirstOrDefault(t => t.Id.Equals(selectedTm.Id));
				if (tmToRemove != null)
				{
					tmsList.Remove(tmToRemove);
				}
			}
		}

	    public void RemoveFiles(IList filesToRemove, ObservableCollection<FileDetails> filesList)
	    {
		    var selectedItems = filesToRemove.Cast<FileDetails>().ToList();
			foreach (var selectedFile in selectedItems)
			{
				var fileToRemove = filesList.FirstOrDefault(f => f.Id.Equals(selectedFile.Id));
				if (fileToRemove != null)
				{
					filesList.Remove(fileToRemove);
				}
			}
		}

	    public void AddTmsToGrid(List<string> localPathList, ObservableCollection<TmDetails> tmsList)
	    {
		    foreach (var filePath in localPathList)
		    {
			    var isTm = Path.GetExtension(filePath)?.ToLower().Equals(".sdltm");
			    if (isTm != true) continue;
			    var tmAlreadyAdded = tmsList.Any(p => p.Path.Equals(filePath));
			    if (tmAlreadyAdded) continue;
			    var tm = new FileBasedTranslationMemory(filePath);

			    var tmDetails = new TmDetails
			    {
				    Name = tm.Name,
				    Path = tm.FilePath,
				    SourceLanguage = tm.LanguageDirection.SourceLanguage,
				    TargetLanguage = tm.LanguageDirection.TargetLanguage,
				    SourceFlag = new Language(tm.LanguageDirection.SourceLanguage.Name).GetFlagImage(),
				    TargetFlag = new Language(tm.LanguageDirection.TargetLanguage.Name).GetFlagImage(),
				    Id = Guid.NewGuid().ToString(),
				    TranslationMemory = new FileBasedTranslationMemory(tm.FilePath),
					ImportSummary = new ImportSummary()
			    };
			    tmDetails.FieldsCollection = tmDetails.TranslationMemory.FieldDefinitions;
			    tmDetails.SetCustomFields();
			    tmsList.Add(tmDetails);
		    }
	    }

	    public void AddFilesToGrid(List<string> localPathList, ObservableCollection<FileDetails> filesList)
	    {
		    foreach (var filePath in localPathList)
		    {
			    var fileAlreadyAdded = filesList.Any(p => p.Path.Equals(filePath));
			    if (fileAlreadyAdded) continue;

			    var extension = Path.GetExtension(filePath);
			    if (string.IsNullOrEmpty(extension)) continue;

			    var fileDetails = new FileDetails
			    {
				    Name = Path.GetFileName(filePath),
				    Path = filePath,
				    Id = Guid.NewGuid().ToString()
			    };

			    switch (extension.ToLower())
			    {
				    case ".sdlxliff":
					    fileDetails.FileType = FileTypes.Xliff;
					    break;
				    case ".tmx":
					    fileDetails.FileType = FileTypes.Tmx;
					    break;
			    }

			    fileDetails.SetLanguagePairDetails();
			    filesList.Add(fileDetails);
		    }
	    }

		public List<string> LoadFilesFromFolder(string folderPath, List<string> fileExtensions,bool includeSubdirectory)
	    {
			if (string.IsNullOrEmpty(folderPath)) return new List<string>();
			var filesPath = new List<string>();
		    var searchOption = includeSubdirectory ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

		    foreach (var fileExtension in fileExtensions)
		    {
			    filesPath.AddRange(Directory.GetFiles(folderPath, $"*.{fileExtension}", searchOption).ToList());
		    }
		    return filesPath;
		}
    }
}
