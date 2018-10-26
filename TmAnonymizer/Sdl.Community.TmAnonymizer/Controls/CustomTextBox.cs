using System.Windows.Documents;

namespace Sdl.Community.SdlTmAnonymizer.Controls
{
	public static class CustomTextBox
	{
		/// <summary>
		/// Gets the exact position of text pointer
		/// </summary>
		/// <param name="start"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static TextPointer GetPoint(TextPointer start, int x)
		{
			var ret = start;
			var i = 0;
			while (i < x && ret != null)
			{
				if (ret.GetPointerContext(LogicalDirection.Backward) ==
				    TextPointerContext.Text ||
				    ret.GetPointerContext(LogicalDirection.Backward) ==
				    TextPointerContext.None)
					i++;
				if (ret.GetPositionAtOffset(1,
					    LogicalDirection.Forward) == null)
					return ret;
				ret = ret.GetPositionAtOffset(1,
					LogicalDirection.Forward);
			}
			return ret;
		}
	}
}
