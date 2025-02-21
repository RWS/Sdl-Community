using System.Collections.Generic;
using Sdl.Community.PostEdit.Compare.Core;

namespace PostEdit.Compare.Model
{
    /// <summary>
    /// The model interface
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// The interface to the model of the application.
        /// </summary>
        void ImportDataModel(string directoryPathLeft, string directoryPathRight);
        /// <summary>
        /// The pool of data nodes.
        /// </summary>
        List<DataNode> DataPool { get; }

        List<Settings.FileAlignment> FileAlignments { get; set; }

        long TotalFoldersLeft { get; set; }
        long TotalFoldersRight { get; set; }
        long TotalFilesLeft { get; set; }
        long TotalFilesRight { get; set; }
    }
}
