using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using LanguageWeaverProvider.View;
using Xunit;

namespace LanguageWeaverProviderTests.UnitTests
{
    public class ThemedMessageBoxWindowTests
    {
        [Theory]
        [InlineData(MessageBoxButton.OK, "BtnOk", "BtnOk")]
        [InlineData(MessageBoxButton.OKCancel, "BtnOk", "BtnCancel")]
        [InlineData(MessageBoxButton.YesNo, "BtnYes", "BtnNo")]
        [InlineData(MessageBoxButton.YesNoCancel, "BtnYes", "BtnCancel")]
        public void Window_HasEnterAndEscapeButtons(MessageBoxButton button, string defaultButton, string cancelButton)
        {
            var (defaults, cancels) = OnStaThread(() =>
            {
                var buttons = VisibleButtons(Create(button));
                return (Names(buttons.Where(b => b.IsDefault)), Names(buttons.Where(b => b.IsCancel)));
            });

            Assert.Equal(new[] { defaultButton }, defaults);
            Assert.Equal(new[] { cancelButton }, cancels);
        }

        [Theory]
        [InlineData(MessageBoxImage.Warning, true)]
        [InlineData(MessageBoxImage.Question, true)]
        [InlineData(MessageBoxImage.None, false)]
        public void Window_ShowsIconOnlyWhenRequested(MessageBoxImage icon, bool expected)
        {
            var shown = OnStaThread(() =>
            {
                var image = (Image)Create(MessageBoxButton.OK, icon).FindName("ImgIcon");
                return image.Visibility == Visibility.Visible && image.Source != null;
            });

            Assert.Equal(expected, shown);
        }

        [Theory]
        [InlineData(MessageBoxButton.OK, MessageBoxResult.OK)]
        [InlineData(MessageBoxButton.OKCancel, MessageBoxResult.Cancel)]
        [InlineData(MessageBoxButton.YesNo, MessageBoxResult.No)]
        [InlineData(MessageBoxButton.YesNoCancel, MessageBoxResult.Cancel)]
        public void Window_ClosedWithoutButton_ReturnsNativeResult(MessageBoxButton button, MessageBoxResult expected)
        {
            var result = OnStaThread(() => Create(button).Result);

            Assert.Equal(expected, result);
        }

        private static ThemedMessageBoxWindow Create(MessageBoxButton button, MessageBoxImage icon = MessageBoxImage.None) =>
            new ThemedMessageBoxWindow("text", "caption", button, icon);

        private static Button[] VisibleButtons(ThemedMessageBoxWindow window) =>
            new[] { "BtnYes", "BtnOk", "BtnNo", "BtnCancel" }
                .Select(name => (Button)window.FindName(name))
                .Where(b => b.Visibility == Visibility.Visible)
                .ToArray();

        private static string[] Names(IEnumerable<Button> buttons) => buttons.Select(b => b.Name).ToArray();

        private static T OnStaThread<T>(Func<T> action)
        {
            T result = default;
            Exception error = null;
            var thread = new Thread(() =>
            {
                try { result = action(); }
                catch (Exception e) { error = e; }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            if (error != null) throw new Exception("STA thread failed", error);
            return result;
        }
    }
}
