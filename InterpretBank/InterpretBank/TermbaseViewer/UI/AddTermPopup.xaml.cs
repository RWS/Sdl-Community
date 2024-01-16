using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Point = System.Windows.Point;

namespace InterpretBank.TermbaseViewer.UI
{
    /// <summary>
    /// Interaction logic for AddTermPopup.xaml
    /// </summary>
    public partial class AddTermPopup : Window
    {
        public static DependencyProperty SelectedGlossaryProperty = DependencyProperty.Register(nameof(SelectedGlossary), typeof(string), typeof(AddTermPopup), new PropertyMetadata(default(string)));
        public static DependencyProperty GlossariesProperty = DependencyProperty.Register(nameof(Glossaries), typeof(List<string>), typeof(AddTermPopup), new PropertyMetadata(default(List<string>)));
        public static DependencyProperty SourceLanguageProperty = DependencyProperty.Register(nameof(SourceLanguage), typeof(string), typeof(AddTermPopup), new PropertyMetadata(default(string)));
        public static DependencyProperty SourceTermProperty = DependencyProperty.Register(nameof(SourceTerm), typeof(string), typeof(AddTermPopup), new PropertyMetadata(default(string)));
        public static DependencyProperty TargetLanguageProperty = DependencyProperty.Register(nameof(TargetLanguage), typeof(string), typeof(AddTermPopup), new PropertyMetadata(default(string)));
        public static DependencyProperty TargetTermProperty = DependencyProperty.Register(nameof(TargetTerm), typeof(string), typeof(AddTermPopup), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty SourceLanguageFlagProperty = DependencyProperty.Register(nameof(SourceLanguageFlag), typeof(Image), typeof(AddTermPopup), new PropertyMetadata(default(Image)));
        public static readonly DependencyProperty TargetLanguageFlagProperty = DependencyProperty.Register(nameof(TargetLanguageFlag), typeof(Image), typeof(AddTermPopup), new PropertyMetadata(default(Image)));

        public AddTermPopup()
        {
            InitializeComponent();
            Deactivated += SettingsUi_Deactivated;
            PreviewKeyDown += SettingsUi_PreviewKeyDown;
        }

        public string SelectedGlossary
        {
            get => (string)GetValue(SelectedGlossaryProperty);
            set => SetValue(SelectedGlossaryProperty, value);
        }

        public List<string> Glossaries
        {
            get => (List<string>)GetValue(GlossariesProperty);
            set => SetValue(GlossariesProperty, value);
        }

        public string SourceLanguage
        {
            get => (string)GetValue(SourceLanguageProperty);
            set => SetValue(SourceLanguageProperty, value);
        }

        public string SourceTerm
        {
            get => (string)GetValue(SourceTermProperty);
            set => SetValue(SourceTermProperty, value);
        }

        public string TargetLanguage
        {
            get => (string)GetValue(TargetLanguageProperty);
            set => SetValue(TargetLanguageProperty, value);
        }

        public string TargetTerm
        {
            get => (string)GetValue(TargetTermProperty);
            set => SetValue(TargetTermProperty, value);
        }

        public Image SourceLanguageFlag
        {
            get => (Image)GetValue(SourceLanguageFlagProperty);
            set => SetValue(SourceLanguageFlagProperty, value);
        }

        public Image TargetLanguageFlag
        {
            get => (Image)GetValue(TargetLanguageFlagProperty);
            set => SetValue(TargetLanguageFlagProperty, value);
        }

        public Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        private void SettingsUi_Deactivated(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch{}
        }

        private void SettingsUi_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Deactivated -= SettingsUi_Deactivated;
                Close();
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            MoveBottomRightEdgeOfWindowToMousePosition();
        }

        private void MoveBottomRightEdgeOfWindowToMousePosition()
        {
            var transform = PresentationSource.FromVisual(this)?.CompositionTarget.TransformFromDevice;
            if (transform is null)
                return;

            var mouse = transform.Value.Transform(GetMousePosition());
            Left = mouse.X - ActualWidth / 2;
            Top = mouse.Y;
        }

        //public string SelectedGlossary { get; set; }
        //public string SourceLanguage { get; set; }
        //public string SourceTerm { get; set; }
        //public string TargetLanguage { get; set; }
        //public string TargetTerm { get; set; }
        //public List<string> Glossaries { get; set; }

        private void AddTermButton_OnClick(object sender, RoutedEventArgs e)
        {
            TermAdded = true;
            Close();
        }

        public bool TermAdded { get; set; }
    }
}