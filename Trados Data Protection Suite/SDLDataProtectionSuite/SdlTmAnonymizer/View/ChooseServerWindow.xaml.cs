using Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.Model;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Sdl.Community.SdlDataProtectionSuite.SdlTmAnonymizer.View
{
    /// <summary>
    /// Interaction logic for ChooseServerWindow.xaml
    /// </summary>
    public partial class ChooseServerWindow
    {
        public static readonly DependencyProperty ServersProperty = DependencyProperty.Register(nameof(Servers),
            typeof(List<ServerItem>), typeof(ChooseServerWindow), new PropertyMetadata(default(
                List<ServerItem>)));

        public ChooseServerWindow(List<Uri> servers)
        {
            Initialize(servers);
            DataContext = this;
            InitializeComponent();
        }

        public List<ServerItem> Servers
        {
            get => (List<ServerItem>)GetValue(ServersProperty);
            set => SetValue(ServersProperty, value);
        }

        private void Initialize(List<Uri> servers)
        {
            Servers = [];
            foreach (var server in servers)
                Servers.Add(new ServerItem { Uri = server.ToString() });
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}