using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI.Controls
{
    /// <summary>
    /// Interaction logic for TranslationMemories.xaml
    /// </summary>
    public partial class TranslationMemories : UserControl
    {

        public TranslationMemories(TranslationMemoriesViewModel tmViewModel)
        {

            InitializeComponent();
            DataContext = tmViewModel;

        }

        public bool TmFieldIsCompleted()
        {
            bool isCompleted = !(Create.IsChecked == true && string.IsNullOrEmpty(TmName.Text));
            if (Browse.IsChecked == true && string.IsNullOrEmpty(TmName.Text))
            {
                isCompleted= false;
            }
            return isCompleted;
        }
     
    }
}
