using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Sdl.Community.SdlDataProtectionSuite.SdlProjectAnonymizer.Helpers
{
	public delegate void CheckBoxHeaderClickHandler(CheckBoxHeaderCellEventArgs e);

	public class CustomColumnHeader : DataGridViewColumnHeaderCell
	{
		public bool IsChecked;
		public CheckBoxState CheckboxState = CheckBoxState.UncheckedNormal;
		private Size _checkboxsize;
		private Point _location;
		private Point _cellboundsLocation;

		public CustomColumnHeader()
		{
			_location = new Point();
			_cellboundsLocation = new Point();
			IsChecked = false;
		}

		public event CheckBoxHeaderClickHandler OnCheckBoxHeaderClick;
		protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
		{
			/* Make a condition to check whether the click is fired inside a checkbox region */
			var clickpoint = new Point(e.X + _cellboundsLocation.X, e.Y + _cellboundsLocation.Y);

			if ((clickpoint.X > _location.X && clickpoint.X < (_location.X + _checkboxsize.Width)) && (clickpoint.Y > _location.Y && clickpoint.Y < (_location.Y + _checkboxsize.Height)))
			{
				IsChecked = !IsChecked;
				if (OnCheckBoxHeaderClick != null)
				{
					OnCheckBoxHeaderClick(new CheckBoxHeaderCellEventArgs(IsChecked));
					this.DataGridView.InvalidateCell(this);
				}
			}
			base.OnMouseClick(e);
		}

		protected override void Paint(Graphics graphics, Rectangle clipBounds,
			Rectangle cellBounds, int rowIndex, DataGridViewElementStates dataGridViewElementState, object value, object formattedValue, string errorText,
			DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle
				advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			base.Paint(graphics, clipBounds, cellBounds, rowIndex, dataGridViewElementState,
				value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

			_checkboxsize = CheckBoxRenderer.GetGlyphSize(graphics, CheckBoxState.UncheckedNormal);
			_location.X = cellBounds.X + (cellBounds.Width / 2 - _checkboxsize.Width / 2);
			_location.Y = cellBounds.Y + (cellBounds.Height / 2 - _checkboxsize.Height / 2);
			_cellboundsLocation = cellBounds.Location;

			CheckboxState = IsChecked 
				? CheckBoxState.CheckedNormal 
				: CheckBoxState.UncheckedNormal;

			CheckBoxRenderer.DrawCheckBox(graphics, _location, CheckboxState);
		}
	}
}