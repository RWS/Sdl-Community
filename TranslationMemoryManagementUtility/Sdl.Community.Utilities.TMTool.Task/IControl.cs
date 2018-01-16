using System.Windows.Forms;

namespace Sdl.Community.Utilities.TMTool.Task
{
	public interface IControl
	{
		/// <summary>
		/// settings user control
		/// </summary>
		UserControl UControl { get; }
		/// <summary>
		/// current settings
		/// </summary>
		ISettings Options { get; }
		/// <summary>
		/// reset UI view, set default Options values
		/// </summary>
		void ResetUI();
		/// <summary>
		/// updates UI with settings
		/// </summary>
		/// <param name="settings">settings to update to</param>
		void UpdateUI(ISettings settings);
	}
}