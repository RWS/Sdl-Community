using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Multilingual.Excel.FileType.Controls
{
	public partial class ColorPicker : ComboBox
	{
		// Data for each color in the list
		public class ColorInfo
		{
			public string Text { get; set; }
			public Color Color { get; set; }

			public ColorInfo(string text, Color color)
			{
				Text = text;
				Color = color;
			}
		}

		public ColorPicker()
		{
			InitializeComponent();

			DropDownStyle = ComboBoxStyle.DropDownList;
			DrawMode = DrawMode.OwnerDrawFixed;
			DrawItem += OnDrawItem;

			AddStandardColors();
		}

		// Populate control with standard colors
		private void AddStandardColors()
		{
			Items.Clear();
			
			var colorType = typeof(Color);
			var propInfoList = colorType.GetProperties(BindingFlags.Static |  BindingFlags.DeclaredOnly | BindingFlags.Public);
			foreach (var propertyInfo in propInfoList)
			{
				var name = propertyInfo.Name;
				var color = Color.FromName(name);				

				if (string.Compare(name, "Transparent", StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					Items.Add(new ColorInfo(name, color));
				}
			}
		}

		// Draw list item
		protected void OnDrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.Index < 0)
			{
				return;
			}

			// Get this color
			var color = (ColorInfo)Items[e.Index];

			// Fill background
			e.DrawBackground();

			// Draw color box
			var rect = new Rectangle
			{
				X = e.Bounds.X + 2,
				Y = e.Bounds.Y + 2,
				Width = 18,
				Height = e.Bounds.Height - 5
			};

			e.Graphics.FillRectangle(new SolidBrush(color.Color), rect);
			e.Graphics.DrawRectangle(SystemPens.WindowText, rect);

			// Write color name
			var brush = (e.State & DrawItemState.Selected) != DrawItemState.None 
				? SystemBrushes.HighlightText 
				: SystemBrushes.WindowText;

			e.Graphics.DrawString(color.Text, Font, brush,
				e.Bounds.X + rect.X + rect.Width + 2,
				e.Bounds.Y + ((e.Bounds.Height - Font.Height) / 2));

			// Draw the focus rectangle if appropriate
			if ((e.State & DrawItemState.NoFocusRect) == DrawItemState.None)
			{
				e.DrawFocusRectangle();
			}
		}

		/// <summary>
		/// Gets or sets the currently selected item.
		/// </summary>
		public new ColorInfo SelectedItem
		{
			get => (ColorInfo)base.SelectedItem;
			set => base.SelectedItem = value;
		}

		/// <summary>
		/// Gets the text of the selected item, or sets the selection to
		/// the item with the specified text.
		/// </summary>
		public new string SelectedText
		{
			get => SelectedIndex >= 0 ? SelectedItem.Text : string.Empty;
			set
			{
				for (var i = 0; i < Items.Count; i++)
				{
					if (((ColorInfo)Items[i]).Text == value)
					{
						SelectedIndex = i;
						break;
					}
				}
			}
		}

		/// <summary>
		/// Gets the value of the selected item, or sets the selection to
		/// the item with the specified value.
		/// </summary>
		public new Color SelectedValue
		{
			get => SelectedIndex >= 0 ? SelectedItem.Color : Color.White;
			set
			{
				for (var i = 0; i < Items.Count; i++)
				{
					if (((ColorInfo)Items[i]).Color == value)
					{
						SelectedIndex = i;
						break;
					}
				}
			}
		}
	}
}
