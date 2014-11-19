using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Sdl.Community.TMOptimizer
{
    public class ProcessingStateToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ProcessingState state = (ProcessingState)value;
            switch (state)
            {
                case ProcessingState.Canceled:
                case ProcessingState.Canceling:
                case ProcessingState.Failed:
                    return GetImagePath("error.png");
                case ProcessingState.Completed:
                    return GetImagePath("completed.png");
                case ProcessingState.NotProcessing:
                    return GetImagePath("notprocessing.png");
                case ProcessingState.Processing:
                    return GetImagePath("processing.png");
                default:
                    return null;
            }
        }

        private string GetImagePath(string imageFileName)
        {
            return "Images\\" + imageFileName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
