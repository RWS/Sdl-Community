using Sdl.Community.DsiViewer.Model;
using System.Windows;
using System.Windows.Controls;

namespace Sdl.Community.DsiViewer.View
{
    /// <summary>
    /// Interaction logic for TqeCard.xaml
    /// </summary>
    public partial class TqeCard : UserControl
    {
        public static readonly DependencyProperty TqeDataProperty = DependencyProperty.Register(nameof(TqeData), typeof(TranslationOriginData), typeof(TqeCard), new PropertyMetadata(default(TranslationOriginData)));

        public TqeCard()
        {
            InitializeComponent();
        }

        public TranslationOriginData TqeData
        {
            get => (TranslationOriginData)GetValue(TqeDataProperty);
            set => SetValue(TqeDataProperty, value);
        }
    }
}