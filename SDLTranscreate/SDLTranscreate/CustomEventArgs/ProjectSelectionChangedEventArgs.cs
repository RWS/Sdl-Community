namespace Trados.Transcreate.CustomEventArgs
{
	public class ProjectSelectionChangedEventArgs: System.EventArgs
	{
		public Interfaces.IProject SelectedProject { get; set; }
	}
}
