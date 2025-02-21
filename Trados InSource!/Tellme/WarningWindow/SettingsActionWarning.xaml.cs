using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Sdl.Community.InSource.Tellme.WarningWindow;

/// <summary>
/// Interaction logic for SettingsActionWarning.xaml
/// </summary>
public partial class SettingsActionWarning : Window
{
    public static readonly DependencyProperty ImagePathProperty = DependencyProperty.Register(nameof(ImagePath), typeof(string), typeof(SettingsActionWarning), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty MessageTitleProperty = DependencyProperty.Register(nameof(MessageTitle), typeof(string), typeof(SettingsActionWarning), new PropertyMetadata(default(string)));
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(SettingsActionWarning), new PropertyMetadata(default(string)));
    private readonly string _url = "https://appstore.rws.com/Plugin/31?tab=documentation";

    public SettingsActionWarning(string text, string title, MessageBoxButton messageBoxButton, MessageBoxImage messageBoxImage)
    {
        InitializeComponent();

        SetButtons(messageBoxButton);
        SetImage(messageBoxImage);
        MessageTitle = title;
        Text = text;
    }

    public string ImagePath
    {
        get => (string)GetValue(ImagePathProperty);
        set => SetValue(ImagePathProperty, value);
    }

    public string MessageTitle
    {
        get => (string)GetValue(MessageTitleProperty);
        set => SetValue(MessageTitleProperty, value);
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

    private void SetButtons(MessageBoxButton messageBoxButton)
    {
        if (messageBoxButton != MessageBoxButton.OKCancel)
            return;
        OkButton.Visibility = Visibility.Visible;
        CloseButton.Content = "Cancel";
    }

    private void SetImage(MessageBoxImage messageBoxImage)
    {
        ImagePath = messageBoxImage switch
        {
            MessageBoxImage.Information => "/Sdl.Community.InSource;component/Resources/information.png",
            MessageBoxImage.Warning => "/Sdl.Community.InSource;component/Resources/warning.png",
            MessageBoxImage.Question => "/Sdl.Community.InSource;component/Resources/question.png",
            _ => ImagePath
        };
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

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}