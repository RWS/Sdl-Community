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
    /// Interaction logic for ReturnFiles.xaml
    /// </summary>
    public partial class ReturnFiles : UserControl
    {
        
        public ReturnFiles(ReturnFilesViewModel returnFilesViewModel)
        {
            DataContext = returnFilesViewModel;
            InitializeComponent();
        }
       
    }
}
