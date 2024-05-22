using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.RecordSourceTU.TellMe.WarningWindow;

/// <summary>
/// Interaction logic for SettingsActionWarning.xaml
/// </summary>
public partial class SettingsActionWarning : Window
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(SettingsActionWarning), new PropertyMetadata(default(string)));
    private readonly string _url = "https://appstore.rws.com/Plugin/36?tab=documentation";

    public SettingsActionWarning(string text)
    {
        InitializeComponent();
        Text = text;
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void CloseWindow_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OpenUrl_ButtonClicked(object sender, MouseButtonEventArgs e)
    {
        Process.Start(_url);
    }

    private void OpenUrl_KeyPressed(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter
            || e.Key == Key.Space)
        {
            Process.Start(_url);
        }
    }

    private void SettingsActionWarning_OnLoaded(object sender, RoutedEventArgs e)
    {
        Top = (SystemParameters.PrimaryScreenHeight - ActualHeight) / 2;
        Left = (SystemParameters.PrimaryScreenWidth - ActualWidth) / 2;
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        try
        {
            if (GetWindow(this) is Window window)
            {
                window.DragMove();
            }
        }
        catch { }
    }
}