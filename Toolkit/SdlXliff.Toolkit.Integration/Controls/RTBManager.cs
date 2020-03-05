using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SdlXliff.Toolkit.Integration.Data;

namespace SdlXliff.Toolkit.Integration.Controls
{
	public class RTBManager
	{
		private static Color _matchNotSelected = Color.Yellow;
		private static Color _matchNotSelectedAlternating = Color.LightBlue;
		private static Color _matchSelected = Color.DarkGray;
		private static Color _matchSelectedAlternating = Color.Gray;
		/// <summary>
		/// get color depending on parameters
		/// </summary>
		/// <param name="isSelected">true - control is in selected cell</param>
		/// <param name="isAlternating">true - currect match is alternating</param>
		/// <returns></returns>
		public static Color GetSelectionColor(bool isSelected, bool isAlternating)
		{
			if (isSelected && !isAlternating)
				return _matchSelected;
			else if (!isSelected && !isAlternating)
				return _matchNotSelected;
			else if (!isSelected && isAlternating)
				return _matchNotSelectedAlternating;
			else // (isSelected && isAlternating)
				return _matchSelectedAlternating;
		}

		/// <summary>
		/// select match text with backcolor (alternating) depending on parameters
		/// </summary>
		/// <param name="ctl">RTB control</param>
		/// <param name="ddata"></param>
		/// <param name="isSelected">true - control is in selected cell</param>
		public static void SelectRTBSegmentText(RichTextBox ctl, List<IndexData> dataMatches, bool isSelected)
		{
			for (int i = 0; i < dataMatches.Count; i++)
			{
				var index = dataMatches[i].IndexStart;
				if (dataMatches[i].Length != 0 && ctl.Text[index] == '\n')
				{
					index -= 1;
				}
				ctl.Select(index, dataMatches[i].Length);

				ctl.SelectionBackColor = (i % 2 == 0 ?
					GetSelectionColor(isSelected, false) : GetSelectionColor(isSelected, true));
			}
		}
	}
}