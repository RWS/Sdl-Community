using System;
using System.Collections.Generic;
using System.Drawing;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Color = System.Windows.Media.Color;

namespace Multilingual.Excel.FileType.Services
{
	public class ColorService
	{
		const string defaultColor = "#00FFFFFF";
		private Dictionary<int, string> _colorPalette;

		public ColorService()
		{
			_colorPalette = new Dictionary<int, string>
			{
				{ 0, "FFFFFF" },
				{ 1, "000000" },
				{ 2, "EEECE1" },
				{ 3, "1F49FD" },
				{ 4, "4F81BD" },
				{ 5, "C0504D" },
				{ 6, "9BBB59" },
				{ 7, "8064A2" },
				{ 8, "4BACC6" },
				{ 9, "F79646" }
			};
		}

		public string GetColorPalette(int color)
		{
			return _colorPalette[color];
		}

		private struct HlsColor
		{
			public double A;
			public double H;
			public double L;
			public double S;
		}
		private struct HsbColor
		{
			public double A;
			public double H;
			public double S;
			public double B;
		}

		private Color HsbToRgb(HsbColor hsbColor)
		{
			// Initialize
			var rgbColor = new Color();

			/* Gray (zero saturation) is a special case.We simply
             * set RGB values to HSB Brightness value and exit. */

			// Gray: Set RGB and return
			if (hsbColor.S == 0.0)
			{
				rgbColor.A = (byte)(hsbColor.A * 255);
				rgbColor.R = (byte)(hsbColor.B * 255);
				rgbColor.G = (byte)(hsbColor.B * 255);
				rgbColor.B = (byte)(hsbColor.B * 255);
				return rgbColor;
			}

			/* Now we process the normal case. */

			var h = (hsbColor.H == 360) ? 0 : hsbColor.H / 60;
			var i = (int)(Math.Truncate(h));
			var f = h - i;

			var p = hsbColor.B * (1.0 - hsbColor.S);
			var q = hsbColor.B * (1.0 - (hsbColor.S * f));
			var t = hsbColor.B * (1.0 - (hsbColor.S * (1.0 - f)));

			double r, g, b;
			switch (i)
			{
				case 0:
					r = hsbColor.B;
					g = t;
					b = p;
					break;

				case 1:
					r = q;
					g = hsbColor.B;
					b = p;
					break;

				case 2:
					r = p;
					g = hsbColor.B;
					b = t;
					break;

				case 3:
					r = p;
					g = q;
					b = hsbColor.B;
					break;

				case 4:
					r = t;
					g = p;
					b = hsbColor.B;
					break;

				default:
					r = hsbColor.B;
					g = p;
					b = q;
					break;
			}

			// Set WPF Color object
			rgbColor.A = (byte)(hsbColor.A * 255);
			rgbColor.R = (byte)(r * 255);
			rgbColor.G = (byte)(g * 255);
			rgbColor.B = (byte)(b * 255);

			// Set return value
			return rgbColor;
		}

		private HsbColor RgbToHsb(Color rgbColor)
		{
			/* Hue values range between 0 and 360. All 
             * other values range between 0 and 1. */

			// Create HSB color object
			var hsbColor = new HsbColor();

			// Get RGB color component values
			var r = (int)rgbColor.R;
			var g = (int)rgbColor.G;
			var b = (int)rgbColor.B;
			var a = (int)rgbColor.A;

			// Get min, max, and delta values
			double min = Math.Min(Math.Min(r, g), b);
			double max = Math.Max(Math.Max(r, g), b);
			double delta = max - min;

			/* Black (max = 0) is a special case. We 
             * simply set HSB values to zero and exit. */

			// Black: Set HSB and return
			if (max == 0.0)
			{
				hsbColor.H = 0.0;
				hsbColor.S = 0.0;
				hsbColor.B = 0.0;
				hsbColor.A = a;
				return hsbColor;
			}

			/* Now we process the normal case. */

			// Set HSB Alpha value
			var alpha = (double)a;
			hsbColor.A = alpha / 255;

			// Set HSB Hue value
			if (r == max) hsbColor.H = (g - b) / delta;
			else if (g == max) hsbColor.H = 2 + (b - r) / delta;
			else if (b == max) hsbColor.H = 4 + (r - g) / delta;
			hsbColor.H *= 60;
			if (hsbColor.H < 0.0) hsbColor.H += 360;

			// Set other HSB values
			hsbColor.S = delta / max;
			hsbColor.B = max / 255;

			// Set return value
			return hsbColor;
		}

		private HlsColor RgbToHls(Color rgbColor)
		{
			// Initialize result
			var hlsColor = new HlsColor();

			// Convert RGB values to percentages
			double r = (double)rgbColor.R / 255;
			var g = (double)rgbColor.G / 255;
			var b = (double)rgbColor.B / 255;
			var a = (double)rgbColor.A / 255;

			// Find min and max RGB values
			var min = Math.Min(r, Math.Min(g, b));
			var max = Math.Max(r, Math.Max(g, b));
			var delta = max - min;

			/* If max and min are equal, that means we are dealing with 
             * a shade of gray. So we set H and S to zero, and L to either
             * max or min (it doesn't matter which), and  then we exit. */

			//Special case: Gray
			if (max == min)
			{
				hlsColor.H = 0;
				hlsColor.S = 0;
				hlsColor.L = max;
				return hlsColor;
			}

			/* If we get to this point, we know we don't have a shade of gray. */

			// Set L
			hlsColor.L = (min + max) / 2;

			// Set S
			if (hlsColor.L < 0.5)
			{
				hlsColor.S = delta / (max + min);
			}
			else
			{
				hlsColor.S = delta / (2.0 - max - min);
			}

			// Set H
			if (r == max) hlsColor.H = (g - b) / delta;
			if (g == max) hlsColor.H = 2.0 + (b - r) / delta;
			if (b == max) hlsColor.H = 4.0 + (r - g) / delta;
			hlsColor.H *= 60;
			if (hlsColor.H < 0) hlsColor.H += 360;

			// Set A
			hlsColor.A = a;

			// Set return value
			return hlsColor;

		}

		
		/// <summary>
		/// Converts a WPF HSL color to an RGB color
		/// </summary>
		/// <param name="hlsColor">The HSL color to convert.</param>
		/// <returns>An RGB color object equivalent to the HSL color object passed in.</returns>
		private Color HlsToRgb(HlsColor hlsColor)
		{
			// Initialize result
			var rgbColor = new  Color();

			/* If S = 0, that means we are dealing with a shade 
             * of gray. So, we set R, G, and B to L and exit. */

			// Special case: Gray
			if (hlsColor.S == 0)
			{
				rgbColor.R = Convert.ToByte(hlsColor.L * 255);
				rgbColor.G = Convert.ToByte(hlsColor.L * 255);
				rgbColor.B = Convert.ToByte(hlsColor.L * 255);
				rgbColor.A = Convert.ToByte(hlsColor.A * 255);
				return rgbColor;
			}

			double t1;
			if (hlsColor.L < 0.5)
			{
				t1 = hlsColor.L * (1.0 + hlsColor.S);
			}
			else
			{
				t1 = hlsColor.L + hlsColor.S - (hlsColor.L * hlsColor.S);
			}

			var t2 = 2.0 * hlsColor.L - t1;

			// Convert H from degrees to a percentage
			var h = hlsColor.H / 360;

			// Set colors as percentage values
			var tR = h + (1.0 / 3.0);
			var r = SetColor(t1, t2, tR);

			var tG = h;
			var g = SetColor(t1, t2, tG);

			var tB = h - (1.0 / 3.0);
			var b = SetColor(t1, t2, tB);

			// Assign colors to Color object
			rgbColor.R = Convert.ToByte(r * 255);
			rgbColor.G = Convert.ToByte(g * 255);
			rgbColor.B = Convert.ToByte(b * 255);
			rgbColor.A = Convert.ToByte(hlsColor.A * 255);

			// Set return value
			return rgbColor;
		}

		private double CalculateFinalLumValue(DoubleValue tint, float lum)
		{
			if (tint == null)
			{
				return lum;
			}

			double lum1;
			if (tint.Value < 0)
			{
				lum1 = lum * (1.0 + tint.Value);
			}
			else
			{
				lum1 = lum * (1.0 - tint.Value) + (255 - 255 * (1.0 - tint.Value));
			}

			return lum1;
		}

		private double SetColor(double t1, double t2, double t3)
		{
			if (t3 < 0) t3 += 1.0;
			if (t3 > 1) t3 -= 1.0;

			double color;
			if (6.0 * t3 < 1)
			{
				color = t2 + (t1 - t2) * 6.0 * t3;
			}
			else if (2.0 * t3 < 1)
			{
				color = t1;
			}
			else if (3.0 * t3 < 2)
			{
				color = t2 + (t1 - t2) * ((2.0 / 3.0) - t3) * 6.0;
			}
			else
			{
				color = t2;
			}

			// Set return value
			return color;
		}

		//Convert.ToInt32(themeValue.Value)
		public string GetColor(ColorType ct, int themeValue)
		{
			var colourValue = GetColorPalette(themeValue);

			var fromHtml = ColorTranslator.FromHtml("#" + colourValue);

			var newColor = Color.FromArgb(0, fromHtml.R, fromHtml.G, fromHtml.B);
			

			var rgbToHls = RgbToHls(newColor);

			double calculateFinalLumValue = CalculateFinalLumValue(ct.Tint, (float)rgbToHls.L * 255) / 255;

			rgbToHls.L = calculateFinalLumValue;

			var hlsToRgb = HlsToRgb(rgbToHls);
			return hlsToRgb.ToString();
		}

		public System.Windows.Media.Color GetColor(string name)
		{
			try
			{
				if (!string.IsNullOrEmpty(name))
				{
					var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(name);
					return color;
				}
			}
			catch
			{
				// catch all; ignore
			}

			return (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(defaultColor);
		}

		public string GetColorHexCode(System.Windows.Media.Color color)
		{
			return color.ToString();
		}
	}
}
