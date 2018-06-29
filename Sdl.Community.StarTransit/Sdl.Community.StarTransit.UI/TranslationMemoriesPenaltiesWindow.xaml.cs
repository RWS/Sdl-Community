using System.Windows;
using Sdl.Community.StarTransit.UI.ViewModels;

namespace Sdl.Community.StarTransit.UI
{
	/// <summary>
	/// Interaction logic for TranslationMemoriesPenaltiesWindow.xaml
	/// </summary>
	public partial class TranslationMemoriesPenaltiesWindow : Window
    {
        public TranslationMemoriesPenaltiesWindow(TranslationMemoriesPenaltiesViewModel tmPenaltiesViewModel)
        {
            InitializeComponent();
			DataContext = tmPenaltiesViewModel;
        }
    }
}