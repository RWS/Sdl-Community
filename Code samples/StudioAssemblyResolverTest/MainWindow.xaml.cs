using System.Collections.Generic;
using System.Windows;
using Rws.StudioAssemblyResolver;
using Rws.StudioAssemblyResolver.PathResolver;

namespace StudioAssemblyResolverTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            AssemblyResolver
                .WithPathResolver(new List<IPathResolver> { new RegistryStudio2022PathResolver() })
                .Resolve();
            Registry.Text = AssemblyResolver.StudioPath;

            AssemblyResolver
                .WithPathResolver(new List<IPathResolver> { new Studio2022PathResolver() })
                .Resolve();
            Filepath.Text = AssemblyResolver.StudioPath;

            AssemblyResolver.Resolve();
            Latest.Text = AssemblyResolver.StudioPath;
        }
    }
}