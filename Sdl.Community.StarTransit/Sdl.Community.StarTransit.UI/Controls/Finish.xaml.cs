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
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;

namespace Sdl.Community.StarTransit.UI.Controls
{
    /// <summary>
    /// Interaction logic for Finish.xaml
    /// </summary>
    public partial class Finish : UserControl
    {
        private readonly PackageModel _package;
        private readonly ProjectService _projectService;
        public Finish(PackageModel package)
        {
           
            _projectService = new ProjectService();
            _package = package;
            InitializeComponent();
        }

        private void CreateProjectBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //primesc eroare la InitializeComponent() daca referentiez sdl.filebased(ca sa pot returna un proiect creat in serviciu)
            _projectService.CreateProject(_package);
            var messageBox = MessageBox.Show("Project created");
        }
    }
}
