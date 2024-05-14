using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Trados.Transcreate.TellMe.WarningWindow;

/// <summary>
/// Interaction logic for SettingsActionWarning.xaml
/// </summary>
public partial class SettingsActionWarning : Window
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(SettingsActionWarning), new PropertyMetadata(default(string)));
    private readonly string _url = "https://appstore.rws.com/Plugin/42?tab=documentation";

    public SettingsActionWarning(string text, int size)
    {
        Width = Size == 0 ? 400 : 550;
        InitializeComponent();
        Text = text;
    }

    public int Size { get; }

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

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Close();
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (IsFocused && GetWindow(this) is Window window)
        {
            window.DragMove();
        }
    }
}