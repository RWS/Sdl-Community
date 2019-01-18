using System.ComponentModel;
using Sdl.Community.TMOptimizerLib;

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
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

    }
}