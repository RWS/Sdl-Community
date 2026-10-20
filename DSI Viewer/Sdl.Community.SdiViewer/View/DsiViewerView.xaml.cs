using System.Windows.Controls;
using System.Windows.Media;
using Sdl.Desktop.IntegrationApi.Interfaces;

namespace Sdl.Community.DsiViewer.View
{
	/// <summary>
	/// Interaction logic for DsiViewerView.xaml
	/// </summary>
	public partial class DsiViewerView : UserControl, IUIControl
	{
		// The view uses Studio's Rws.* theme palette, which only builds with the light/dark theme switch provide.
		// On older builds those keys are missing, so supply light-theme values for whichever keys the host lacks;
		// a host that defines them (light, dark or high contrast) always wins.
		private static readonly (string Key, string Color, double Opacity)[] FallbackBrushes =
		{
			("Rws.Bg.Main", "#FFFFFF", 1),
			("Rws.Bg.Top", "#F3F3F3", 1),
			("Rws.Text.Primary", "#1A1A1A", 1),
			("Rws.Text.Muted", "#444444", 1),
			("Rws.Accent.Info", "#0078B4", 1),
			("Rws.Accent.Info.BadgeBg", "#66C9FF", 0.15),
		};

		public DsiViewerView()
		{
			InitializeComponent();
			AddMissingThemeBrushes();
		}

		public void Dispose() { }

		private void AddMissingThemeBrushes()
		{
			foreach (var (key, color, opacity) in FallbackBrushes)
			{
				if (TryFindResource(key) is not null)
				{
					continue;
				}

				var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)) { Opacity = opacity };
				brush.Freeze();
				Resources[key] = brush;
			}
		}
	}
}
