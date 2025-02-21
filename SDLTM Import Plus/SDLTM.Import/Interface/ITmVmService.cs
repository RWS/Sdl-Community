using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SDLTM.Import.Model;

namespace SDLTM.Import.Interface
{
    public interface ITmVmService
    {
	    void RemoveTms(IList tmsToRemove,ObservableCollection<TmDetails> tmsList);
	    void RemoveFiles(IList filesToRemove,ObservableCollection<FileDetails>filesList);
	    List<string> LoadFilesFromFolder(string folderPath,List<string> fileExtension,bool includeSubDirectory);
	    void AddTmsToGrid(List<string> localPathList, ObservableCollection<TmDetails> tmsList);
	    void AddFilesToGrid(List<string>localPathList, ObservableCollection<FileDetails>filesList);
    }
}
