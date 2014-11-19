using Sdl.Community.TMOptimizerLib;
using Sdl.Core.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sdl.Community.TMOptimizer
{
    /// <summary>
    /// A TMX file that needs to be cleaned up.
    /// </summary>
    public class InputTmxFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public InputTmxFile(TmxFile tmxFile)
        {
            TmxFile = tmxFile;
        }

        /// <summary>
        /// The original TMX file
        /// </summary>
        public TmxFile TmxFile
        {
            get;
            private set;
        }

        /// <summary>
        /// The cleaned up TMX file.
        /// </summary>
        public TmxFile CleanTmxFile
        {
            get;
            set;
        }

        private void OnNotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
