using InterpretBank.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace InterpretBank.Converters
{
    public class TermSelector : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var terms = (ObservableCollection<TermModel>)values[0];
            if (terms == null) return null;

            var language = (string)values[1];
            return terms.FirstOrDefault(t => t.LanguageName == language)?.Term;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
