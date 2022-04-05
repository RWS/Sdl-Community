using Sdl.Community.BetaAPIs.UI.DataProvider;
using Sdl.Community.BetaAPIs.UI.DesignTimeData;
using System.ComponentModel;
using System.Windows;

namespace Sdl.Community.BetaAPIs.UI.ViewModel
{
    public class ViewModelLocator
    {
        private MainViewModel _mainViewModel;

        public MainViewModel MainViewModel
        {
            get
            {
                if (_mainViewModel == null)
                {
                    IAPIDataProvider dataProvider =
                        DesignerProperties.GetIsInDesignMode(new FrameworkElement())
                        ? (IAPIDataProvider)new DesignTimeDataProvider()
                        : new JsonDataProvider();
                    _mainViewModel = new MainViewModel(dataProvider);
                }
                return _mainViewModel;
            }
        }
    }
}
